using Extensions.Components.UI;
using Extensions.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Type = SerializableWrapper<Quantum.Weapon>;

public class WeaponPopulator : Populate<Type, long>
{
    [SerializeField] private Type.CreationType _loadType = Type.CreationType.BuiltIn | Type.CreationType.Custom;

    [SerializeField] private bool _showCheckmarks;
    [SerializeField] private int _type;

    private GameObject _oldSelected;

    private const string FILE_PATH = "DB/Assets/Build/Equipment/Weapons/Weapons/Weapons";

    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll()
    {
        List<Type> results = new();

        if (_loadType.HasFlag(Type.CreationType.Custom))
            results.AddRange(FusionFighters.Serializer.LoadAllFromDirectory<Type>(WeaponController.GetPath()));

        if (_loadType.HasFlag(Type.CreationType.BuiltIn))
            results.AddRange(Resources.LoadAll<WeaponAssetAsset>(FILE_PATH).Where(item => item.IncludeInLists).Select(item => item.Weapon));

        return results;
    }

    protected override string Name(Type item) => item.Name;

    protected override Func<Type, long> Sort() => (build) => build.LastEditedDate;

    private bool HasEquipped(Type item)
    {
        return _type switch
        {
            0 => BuildController.Instance.CurrentlySelected.value.Equipment.Weapons.MainWeapon.Equals(item.value),
            1 => BuildController.Instance.CurrentlySelected.value.Equipment.Weapons.AltWeapon.Equals(item.value),
            _ => false,
        };
    }

    protected override void Decorate(GameObject buttonObj, Type item)
    {
        base.Decorate(buttonObj, item);

        if (!_showCheckmarks)
            return;

        bool isEquipped = HasEquipped(item);
        buttonObj.FindChildWithTag("Checkmark", true)?.SetActive(isEquipped);

        if (isEquipped)
        {
            _oldSelected = buttonObj;
        }
    }

    protected override void SetEvents(GameObject buttonObj, Type item)
    {
        base.SetEvents(buttonObj, item);

        if (!_showCheckmarks)
            return;

        Button button = buttonObj.GetComponentInChildren<Button>();
        if (button && GiveEvents(item))
        {
            button.onClick.AddListener(() =>
            {
                if (_oldSelected == buttonObj)
                    return;

                buttonObj.FindChildWithTag("Checkmark", true)?.SetActive(true);

                if (_oldSelected)
                    _oldSelected.FindChildWithTag("Checkmark", true)?.SetActive(false);

                _oldSelected = buttonObj;
            });
        }
    }
}
