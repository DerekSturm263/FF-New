using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class SubController : Controller<SubController>
{
    private SubTemplateAsset _template;
    public void SetTemplate(SubTemplateAsset template) => _template = template;
    public void ClearTemplate() => _template = null;

    private SubEnhancerAsset _enhancer;
    public void SetEnhancer(SubEnhancerAsset enhancer) => _enhancer = enhancer;
    public void ClearEnhancer() => _enhancer = null;

    private string _name = "Untitled";
    public void SetName(string name) => _name = name;
    public void ClearName() => _name = string.Empty;

    private string _description = string.Empty;
    public void SetDescription(string description) => _description = description;
    public void ClearDescription() => _description = string.Empty;

    public void Clear()
    {
        ClearTemplate();
        ClearEnhancer();
        ClearName();
        ClearDescription();
    }

    [SerializeField] private Popup _onSuccess;
    [SerializeField] private Popup _onFail;
    [SerializeField] private Popup _onNotEnoughCurrency;

    [SerializeField] private PopulateBase _populator;
    [SerializeField] private TMPro.TMP_Text _price;

    [SerializeField] private Transform _objParent;
    private GameObject _templateObj;

    [SerializeField] private UnityEvent _onSuccessEvent;

    public static string GetPath() => $"{Application.persistentDataPath}/Subs";

    private void Awake()
    {
        _instance = this;
    }

    public void SaveNew()
    {
        if (!_template)
        {
            PopupController.Instance.DisplayPopup(_onFail);
            return;
        }

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, _enhancer?.Price))
        {
            PopupController.Instance.DisplayPopup(_onNotEnoughCurrency);
            return;
        }

        Sub sub = new()
        {
            Template = new AssetRefSubTemplate() { Id = _template.AssetObject.Guid }
        };
        sub.Enhancers.Enhancer1 = new AssetRefSubEnhancer() { Id = _enhancer ? _enhancer.AssetObject.Guid : AssetGuid.Invalid };

        InventoryController.Instance.UseCountableItem(_template);
        InventoryController.Instance.LoseCurrency(_template.Price);

        if (_enhancer && _enhancer.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_enhancer);
            InventoryController.Instance.LoseCurrency(_enhancer.Price);
        }
        
        SerializableWrapper<Sub> serializable = new(sub, _name, _description, AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Guid, GetPath());

        PopupController.Instance.DisplayPopup(_onSuccess);
        Clear();

        _onSuccessEvent.Invoke();
    }

    private SerializableWrapper<Sub> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Sub> sub) => _currentlySelected = sub;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(SubPopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public void PreviewSub(SerializableWrapper<Sub> sub)
    {
        PreviewTemplate(UnityDB.FindAsset<SubTemplateAsset>(sub.Value.Template.Id));
        PreviewEnhancer(UnityDB.FindAsset<SubEnhancerAsset>(sub.Value.Enhancers.Enhancer1.Id));
    }

    public void PreviewTemplate(SubTemplateAsset template)
    {
        if (_templateObj)
            Destroy(_templateObj);

        if (template.Preview)
            _templateObj = Instantiate(template.Preview, _objParent);

        int price = template.Price;
        _price.SetText($"${price}");

        _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
    }

    public void PreviewEnhancer(SubEnhancerAsset enhancer)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        int price = _template.Price + enhancer.Price;
        _price.SetText($"${price}");

        _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
    }
}
