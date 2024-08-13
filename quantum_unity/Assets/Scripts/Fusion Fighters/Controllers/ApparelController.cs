using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using Extensions.Miscellaneous;
using GameResources.Audio;
using GameResources.UI.Popup;
using Quantum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ApparelController : Controller<ApparelController>
{
    [SerializeField] private AudioClip _jingle;
    [SerializeField] private AudioClip _defaultMusic;

    [SerializeField] private ApparelAssetAsset _none;
    public ApparelAssetAsset None => _none;

    [SerializeField] private ApparelModifierAsset _noneMod;

    [SerializeField] private Camera _renderCamera;
    [SerializeField] private Shader _renderShader;

    private ApparelTemplateAsset _template;
    public void SetTemplate(ApparelTemplateAsset template) => _template = template;
    public void ClearTemplate() => _template = null;

    private Extensions.Types.Dictionary<ApparelModifierAsset, int> _modifiers = new();

    public List<ApparelModifierAsset> GetModifierList()
    {
        List<ApparelModifierAsset> modifiers = new();

        foreach (var kvp in _modifiers)
        {
            for (int i = 0; i < kvp.Value; ++i)
                modifiers.Add(kvp.Key);
        }

        return modifiers;
    }

    public ApparelModifierAsset GetModifierFromIndex(List<ApparelModifierAsset> modifiers, int index)
    {
        if (index >= modifiers.Count)
            return _noneMod;

        return modifiers[index];
    }

    public void AdjustModifiers(ApparelModifierAsset modifier, int increment)
    {
        if ((increment > 0 && _modifiers.Sum(item => item.Value) == 3))
            return;

        if (!_modifiers.ContainsKey(modifier))
            _modifiers.Add(modifier, 0);

        if (_modifiers[modifier] + increment > InventoryController.Instance.GetItemCount(modifier))
            return;

        _modifiers[modifier] += increment;

        if (_modifiers[modifier] >= 0 && ApparelModifierPopulator.Instance && ApparelModifierPopulator.Instance.TryGetButtonFromItem(modifier, out GameObject button))
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
    public void ClearName() => _name = "Untitled";

    private string _description = string.Empty;
    public void SetDescription(string description) => _description = description;
    public void ClearDescription() => _description = string.Empty;

    public void Clear()
    {
        ClearTemplate();
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
        if (!_template || _name == string.Empty)
        {
            PopupController.Instance.Spawn(_onFail);
            return;
        }

        List<ApparelModifierAsset> modifiers = GetModifierList();

        ApparelModifierAsset modifier1 = modifiers.ElementAtOrDefault(0);
        ApparelModifierAsset modifier2 = modifiers.ElementAtOrDefault(1);
        ApparelModifierAsset modifier3 = modifiers.ElementAtOrDefault(2);

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, modifier1?.Price, modifier2?.Price, modifier3?.Price))
        {
            PopupController.Instance.Spawn(_onNotEnoughCurrency);
            return;
        }

        Apparel apparel = new()
        {
            Template = new AssetRefApparelTemplate() { Id = _template.AssetObject.Guid },
            Modifiers = new()
            {
                Modifier1 = new AssetRefApparelModifier() { Id = modifier1 ? modifier1.AssetObject.Guid : AssetGuid.Invalid },
                Modifier2 = new AssetRefApparelModifier() { Id = modifier2 ? modifier2.AssetObject.Guid : AssetGuid.Invalid },
                Modifier3 = new AssetRefApparelModifier() { Id = modifier3 ? modifier3.AssetObject.Guid : AssetGuid.Invalid }
            },
            FileGuid = AssetGuid.NewGuid()
        };

        InventoryController.Instance.LoseItem(_template, 1);
        InventoryController.Instance.LoseCurrency(_template.Price);

        if (InventoryController.Instance.LoseItem(modifier1, 1))
        {
            InventoryController.Instance.LoseCurrency(modifier1.Price);
        }

        if (InventoryController.Instance.LoseItem(modifier2, 1))
        {
            InventoryController.Instance.LoseCurrency(modifier2.Price);
        }

        if (InventoryController.Instance.LoseItem(modifier3, 1))
        {
            InventoryController.Instance.LoseCurrency(modifier3.Price);
        }

        List<string> filterTags = new()
        {
            _template.name
        };

        if (modifiers.Count > 0)
            filterTags.Add(modifiers[0].name);
        if (modifiers.Count > 1)
            filterTags.Add(modifiers[1].name);
        if (modifiers.Count > 2)
            filterTags.Add(modifiers[2].name);

        List<Extensions.Types.Tuple<string, string>> groupTags = new()
        {
            new("Template Type", _template.name)
        };

        SerializableWrapper<Apparel> serializable = new(apparel, GetPath(), _name, _description, apparel.FileGuid, filterTags.ToArray(), groupTags.ToArray());
        serializable.Save();

        _renderCamera.transform.position = _template.IconCameraPosition;
        _renderCamera.transform.rotation = Quaternion.Euler(_template.IconCameraRotation);

        Helper.Delay(1.8f, () => AudioController.Instance.PlayAudioClipAsTrack(_jingle));
        Helper.Delay(10.8f, () => AudioController.Instance.PlayAudioClipAsTrack(_defaultMusic));

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

        _lastApparel.CreateIcon(_renderCamera, _renderShader);

        PopupController.Instance.Spawn(_onSuccess);
        _onSuccessEventDelayed.Invoke(_lastApparel);
    }

    private static SerializableWrapper<Apparel> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Apparel> apparel) => _currentlySelected = apparel;

    private static string _newName = "New Name";

    public void InstanceRename(string name) => _newName = name;

    public void InstanceSubmit() => Instance.Submit();

    public void Submit()
    {
        _currentlySelected.SetName(_newName);
        _currentlySelected.Save();

        if (ApparelPopulator.Instance && ApparelPopulator.Instance.TryGetButtonFromItem(_currentlySelected, out GameObject button))
        {
            ApparelPopulator.Instance.ClearEvents(button);
            ApparelPopulator.Instance.SetEvents(button, _currentlySelected);
        }

        FindFirstObjectByType<DisplayApparel>(FindObjectsInactive.Exclude).UpdateDisplay(_currentlySelected);
    }

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        InventoryController.Instance.GainItem(_currentlySelected.value.Template.Id, 1);

        if (_currentlySelected.value.Modifiers.Modifier1.Id.IsValid)
            InventoryController.Instance.GainItem(_currentlySelected.value.Modifiers.Modifier1.Id, 1);
    
        if (_currentlySelected.value.Modifiers.Modifier2.Id.IsValid)
            InventoryController.Instance.GainItem(_currentlySelected.value.Modifiers.Modifier2.Id, 1);
        
        if (_currentlySelected.value.Modifiers.Modifier3.Id.IsValid)
            InventoryController.Instance.GainItem(_currentlySelected.value.Modifiers.Modifier3.Id, 1);

        foreach (var build in FusionFighters.Serializer.LoadAllFromDirectory<SerializableWrapper<Build>>(BuildController.GetPath()))
        {
            var newBuild = build;

            if (build.value.Outfit.Headgear.Equals(_currentlySelected))
                newBuild.value.Outfit.Headgear = _none.Apparel;
            if (build.value.Outfit.Clothing.Equals(_currentlySelected))
                newBuild.value.Outfit.Clothing = _none.Apparel;
            if (build.value.Outfit.Legwear.Equals(_currentlySelected))
                newBuild.value.Outfit.Legwear = _none.Apparel;

            newBuild.Save();
        }

        _currentlySelected.Delete();

        if (ApparelPopulator.Instance && ApparelPopulator.Instance.TryGetButtonFromItem(_currentlySelected, out GameObject button))
        {
            DestroyImmediate(button);
            ApparelPopulator.Instance.GetComponentInParent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
        }

        ToastController.Instance.Spawn("Apparel deleted");

        Extensions.Miscellaneous.Helper.Delay(0.1f, () => _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First));
    }

    public void PreviewApparel(SerializableWrapper<Apparel> apparel)
    {
        PreviewTemplate(UnityDB.FindAsset<ApparelTemplateAsset>(apparel.value.Template.Id));
    }

    public void PreviewTemplate(ApparelTemplateAsset template)
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

    public void PreviewModifier()
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        if (_price.isActiveAndEnabled)
        {
            uint price = _template.Price + (uint)_modifiers.Sum(item => item.Key.Price * item.Value);
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
