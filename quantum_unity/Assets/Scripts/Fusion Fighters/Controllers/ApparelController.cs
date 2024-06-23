using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ApparelController : Controller<ApparelController>
{
    private ApparelTemplateAsset _template;
    public void SetTemplate(ApparelTemplateAsset template) => _template = template;
    public void ClearTemplate() => _template = null;

    private ApparelPatternAsset _pattern;
    public void SetPattern(ApparelPatternAsset pattern) => _pattern = pattern;
    public void ClearPattern() => _pattern = null;

    private Extensions.Types.Dictionary<ApparelModifierAsset, int> _modifiers = new();
    public void AdjustModifiers(ApparelModifierAsset modifier, int increment)
    {
        if ((increment > 0 && _modifiers.Sum(item => item.Value) == 3))
            return;

        if (!_modifiers.ContainsKey(modifier))
            _modifiers.Add(modifier, 0);

        if (_modifiers[modifier] + increment > InventoryController.Instance.GetItemCount(modifier))
            return;

        _modifiers[modifier] += increment;

        if (_modifiers[modifier] >= 0 && ApparelModifierPopulator.TryButtonFromItem(modifier, out GameObject button))
        {
            TMPro.TMP_Text[] texts = button.GetComponentsInChildren<TMPro.TMP_Text>();
            int newCount = InventoryController.Instance.GetItemCount(modifier) - _modifiers[modifier];

            texts[0].SetText(newCount.ToString());
            texts[0].color = newCount == 0 ? Color.red : Color.white;

            texts[1].SetText(_modifiers[modifier].ToString());
            texts[1].color = _modifiers[modifier] == 0 ? new(0.7f, 0.7f, 0.7f) : Color.white;
        }
        else if (_modifiers[modifier] <= 0)
        {
            _modifiers.Remove(modifier);
        }
    }

    public void ClearModifiers()
    {
        _modifiers.Clear();
    }

    private string _name = "Untitled";
    public void SetName(string name) => _name = name;
    public void ClearName() => _name = string.Empty;

    private string _description = string.Empty;
    public void SetDescription(string description) => _description = description;
    public void ClearDescription() => _description = string.Empty;

    public void Clear()
    {
        ClearTemplate();
        ClearPattern();
        ClearModifiers();
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

    private SerializableWrapper<Apparel> _lastApparel;
    [SerializeField] private UnityEvent<SerializableWrapper<Apparel>> _onSuccessEventDelayed;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Equipment/Apparel";

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

        List<ApparelModifierAsset> modifiers = new();
        foreach (var kvp in _modifiers)
        {
            for (int i = 0; i < kvp.Value; ++i)
                modifiers.Add(kvp.Key);
        }

        ApparelModifierAsset modifier1 = modifiers.ElementAtOrDefault(0);
        ApparelModifierAsset modifier2 = modifiers.ElementAtOrDefault(1);
        ApparelModifierAsset modifier3 = modifiers.ElementAtOrDefault(2);

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, _pattern?.Price, modifier1?.Price, modifier2?.Price, modifier3?.Price))
        {
            PopupController.Instance.DisplayPopup(_onNotEnoughCurrency);
            return;
        }

        Apparel apparel = new()
        {
            Template = new AssetRefApparelTemplate() { Id = _template.AssetObject.Guid },
            Pattern = new AssetRefApparelPattern() { Id = _pattern ? _pattern.AssetObject.Guid : AssetGuid.Invalid }
        };
        apparel.Modifiers.Modifier1 = new AssetRefApparelModifier() { Id = modifier1 ? modifier1.AssetObject.Guid : AssetGuid.Invalid };
        apparel.Modifiers.Modifier2 = new AssetRefApparelModifier() { Id = modifier2 ? modifier2.AssetObject.Guid : AssetGuid.Invalid };
        apparel.Modifiers.Modifier3 = new AssetRefApparelModifier() { Id = modifier3 ? modifier3.AssetObject.Guid : AssetGuid.Invalid };

        InventoryController.Instance.UseCountableItem(_template);
        InventoryController.Instance.LoseCurrency(_template.Price);

        if (_pattern && _pattern.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_pattern);
            InventoryController.Instance.LoseCurrency(_pattern.Price);
        }

        if (modifier1 && modifier1.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(modifier1);
            InventoryController.Instance.LoseCurrency(modifier1.Price);
        }

        if (modifier2 && modifier2.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(modifier2);
            InventoryController.Instance.LoseCurrency(modifier2.Price);
        }

        if (modifier3 && modifier3.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(modifier3);
            InventoryController.Instance.LoseCurrency(modifier3.Price);
        }

        SerializableWrapper<Apparel> serializable = new(apparel, _name, _description, AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.Value.FileGuid = serializable.Guid;
        serializable.SetIcon(_template.Icon);

        Serializer.Save(serializable, serializable.Guid, GetPath());
        _lastApparel = serializable;

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

        PopupController.Instance.DisplayPopup(_onSuccess);
        _onSuccessEventDelayed.Invoke(_lastApparel);
    }

    private SerializableWrapper<Apparel> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Apparel> apparel) => _currentlySelected = apparel;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        InventoryController.Instance.GainCountableItem(_currentlySelected.Value.Template.Id, 1);
        InventoryController.Instance.GainCountableItem(_currentlySelected.Value.Pattern.Id, 1);

        if (_currentlySelected.Value.Modifiers.Modifier1.Id.IsValid)
            InventoryController.Instance.GainCountableItem(_currentlySelected.Value.Modifiers.Modifier1.Id, 1);
    
        if (_currentlySelected.Value.Modifiers.Modifier2.Id.IsValid)
            InventoryController.Instance.GainCountableItem(_currentlySelected.Value.Modifiers.Modifier2.Id, 1);
        
        if (_currentlySelected.Value.Modifiers.Modifier3.Id.IsValid)
            InventoryController.Instance.GainCountableItem(_currentlySelected.Value.Modifiers.Modifier3.Id, 1);

        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(ApparelPopulator.ButtonFromItem(_currentlySelected));
        Extensions.Miscellaneous.Helper.Delay(0.1f, () => _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First));
    }

    public void PreviewApparel(SerializableWrapper<Apparel> apparel)
    {
        PreviewTemplate(UnityDB.FindAsset<ApparelTemplateAsset>(apparel.Value.Template.Id));
        PreviewPattern(UnityDB.FindAsset<ApparelPatternAsset>(apparel.Value.Pattern.Id));
    }

    public void PreviewTemplate(ApparelTemplateAsset template)
    {
        if (WeaponController.TemplateObj)
            Destroy(WeaponController.TemplateObj);

        if (template.Preview)
            WeaponController.TemplateObj = Instantiate(template.Preview, _objParent);

        if (_price.isActiveAndEnabled)
        {
            int price = template.Price;
            _price.SetText($"${price}");

            _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
        }
    }

    public void PreviewPattern(ApparelPatternAsset material)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        if (_price.isActiveAndEnabled)
        {
            int price = _template.Price + material.Price;
            _price.SetText($"${price}");

            _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
        }
    }

    public void PreviewModifier()
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        if (_price.isActiveAndEnabled)
        {
            int price = _template.Price + _pattern.Price + _modifiers.Sum(item => item.Key.Price * item.Value);
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
