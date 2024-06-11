using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class SubController : Controller<SubController>
{
    private SubTemplateAsset _template;
    public void SetTemplate(SubTemplateAsset template) => _template = template;
    public void ClearTemplate() => _template = null;

    private SubEnhancerAsset _enhancer;
    public void SetEnhancer(SubEnhancerAsset enhancer) => _enhancer = enhancer;
    public void ClearEnhancer() => _enhancer = null;

    public void Clear()
    {
        ClearTemplate();
        ClearEnhancer();
    }

    [SerializeField] private Popup _onSuccess;
    [SerializeField] private Popup _onFail;
    [SerializeField] private Popup _onNotEnoughCurrency;

    [SerializeField] private PopulateBase _populator;

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

        if (!Inventory.Instance.HasEnoughCurrency(_template.Price, _enhancer?.Price))
        {
            PopupController.Instance.DisplayPopup(_onNotEnoughCurrency);
            return;
        }

        Sub sub = new();
        sub.Template = new AssetRefSubTemplate() { Id = _template.AssetObject.Guid };
        sub.Enhancers.Enhancer1 = new AssetRefSubEnhancer() { Id = _enhancer ? _enhancer.AssetObject.Guid : AssetGuid.Invalid };

        InventoryController.Instance.UseCountableItem(_template);
        InventoryController.Instance.LoseCurrency(_template.Price);

        if (_enhancer && _enhancer.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_enhancer);
            InventoryController.Instance.LoseCurrency(_enhancer.Price);
        }
        
        SerializableWrapper<Sub> serializable = new(sub, "New Sub", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Value.SerializableData.Guid, GetPath());

        PopupController.Instance.DisplayPopup(_onSuccess);
        Clear();
    }

    private SerializableWrapper<Sub> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Sub> sub) => _currentlySelected = sub;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Value.SerializableData.Guid}.json", path);

        Destroy(SubPopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }
}
