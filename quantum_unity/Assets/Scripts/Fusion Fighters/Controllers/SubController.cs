using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using System.Collections.Generic;
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
    public void ClearName() => _name = "Untitled";

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

    [SerializeField] private UnityEvent _onSuccessEvent;
    [SerializeField] private float _delayTime = 8;

    private bool _doAction;

    private SerializableWrapper<Sub> _lastSub;
    [SerializeField] private UnityEvent<SerializableWrapper<Sub>> _onSuccessEventDelayed;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Equipment/Subs";

    private void Awake()
    {
        _instance = this;
    }

    public void SaveNew()
    {
        if (!_template || _name == string.Empty)
        {
            PopupController.Instance.Spawn(_onFail);
            return;
        }

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, _enhancer?.Price))
        {
            PopupController.Instance.Spawn(_onNotEnoughCurrency);
            return;
        }

        Sub sub = new()
        {
            Template = new AssetRefSubTemplate() { Id = _template.AssetObject.Guid },
            Enhancer = new AssetRefSubEnhancer() { Id = _enhancer ? _enhancer.AssetObject.Guid : AssetGuid.Invalid },
            FileGuid = AssetGuid.NewGuid()
        };

        InventoryController.Instance.LoseItem(_template, 1);
        InventoryController.Instance.LoseCurrency(_template.Price);

        if (InventoryController.Instance.LoseItem(_enhancer, 1))
        {
            InventoryController.Instance.LoseCurrency(_enhancer.Price);
        }

        List<string> filterTags = new()
        {
            _template.name
        };

        if (_enhancer)
            filterTags.Add(_enhancer.name);

        List<Extensions.Types.Tuple<string, string>> groupTags = new()
        {
            new("Template Type", _template.name)
        };

        if (_enhancer)
            groupTags.Add(new("Enhancer Type", _enhancer.name));

        SerializableWrapper<Sub> serializable = new(sub, _name, _description, System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, sub.FileGuid, filterTags.ToArray(), groupTags.ToArray(), _template.Icon, _template.Icon);
        serializable.Save(GetPath());

        _lastSub = serializable;

        Clear();
        _onSuccessEvent.Invoke();

        _doAction = true;
        Invoke(nameof(InvokeEventDelay), _delayTime);
    }

    public void InvokeEventNoDelay()
    {
        InvokeEventDelay();
        _doAction = false;
    }

    private void InvokeEventDelay()
    {
        if (!_doAction)
            return;
            
        PopupController.Instance.Spawn(_onSuccess);
        _onSuccessEventDelayed.Invoke(_lastSub);
    }

    private static SerializableWrapper<Sub> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Sub> sub) => _currentlySelected = sub;

    private static string _newName = "New Name";

    public void InstanceRename(string name) => _newName = name;

    public void InstanceSubmit() => Instance.Submit();

    public void Submit()
    {
        _currentlySelected.SetName(_newName);
        _currentlySelected.Save(GetPath());

        FindFirstObjectByType<DisplaySub>(FindObjectsInactive.Exclude).UpdateDisplay(_currentlySelected);
    }

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        InventoryController.Instance.GainItem(_currentlySelected.value.Template.Id, 1);
        InventoryController.Instance.GainItem(_currentlySelected.value.Enhancer.Id, 1);

        _currentlySelected.Delete(GetPath());

        if (SubPopulator.Instance && SubPopulator.Instance.TryGetButtonFromItem(_currentlySelected, out GameObject button))
            Destroy(button);

        Extensions.Miscellaneous.Helper.Delay(0.1f, () => _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First));
    }

    public void PreviewSub(SerializableWrapper<Sub> sub)
    {
        PreviewTemplate(UnityDB.FindAsset<SubTemplateAsset>(sub.value.Template.Id));
        PreviewEnhancer(UnityDB.FindAsset<SubEnhancerAsset>(sub.value.Enhancer.Id));
    }

    public void PreviewTemplate(SubTemplateAsset template)
    {
        if (WeaponController.TemplateObj)
            Destroy(WeaponController.TemplateObj);

        if (template.Preview)
            WeaponController.TemplateObj = Instantiate(template.Preview, _objParent);

        if (_price.isActiveAndEnabled)
        {
            uint price = template.Price;
            _price.SetText($"${price}");

            _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
        }
    }

    public void PreviewEnhancer(SubEnhancerAsset enhancer)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        if (_price.isActiveAndEnabled)
        {
            uint price = _template.Price + enhancer.Price;
            _price.SetText($"${price}");

            _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
        }
    }

    public void ClearPreview()
    {
        if (WeaponController.TemplateObj)
            Destroy(WeaponController.TemplateObj);
    }
}
