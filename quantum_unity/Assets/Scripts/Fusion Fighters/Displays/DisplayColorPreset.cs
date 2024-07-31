using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = ColorPresetAsset;

public class DisplayColorPreset : DisplayTextAndImage<Type>
{
    public enum ParentType
    {
        Avatar,
        Eyes,
        Hair,
        ApparelHeadgear,
        ApparelClothing,
        ApparelLegwear
    }

    [SerializeField] private ParentType _type;

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue()
    {
        AssetGuid id = _type switch
        {
            ParentType.Avatar => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Headgear.Color.Id,
            ParentType.Eyes => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Headgear.Color.Id,
            ParentType.Hair => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Headgear.Color.Id,
            ParentType.ApparelHeadgear => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Headgear.Color.Id,
            ParentType.ApparelClothing => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Clothing.Color.Id,
            ParentType.ApparelLegwear => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Legwear.Color.Id,
            _ => default
        };

        return UnityDB.FindAsset<Type>(id);
    }
}
