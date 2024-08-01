using Extensions.Miscellaneous;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresetPopulator : PopulateAsset<ColorPresetAsset>
{
    [SerializeField] private DisplayColorPreset.ParentType _type;

    protected override string FilePath() => "DB/Assets/Build/Color Presets";

    protected override bool DoSpawn(ColorPresetAsset item) => InventoryController.Instance.HasUnlockedItem(item);

    protected override Func<ColorPresetAsset, int> Sort() => (item) => -(item.Settings_ColorPreset.Color.R + item.Settings_ColorPreset.Color.G + item.Settings_ColorPreset.Color.B);

    protected override void Decorate(GameObject buttonObj, ColorPresetAsset item)
    {
        base.Decorate(buttonObj, item);

        buttonObj.FindChildWithTag("Icon", false).GetComponent<Image>().color = item.Settings_ColorPreset.Color.ToColor();
    }

    protected override bool HasEquipped(ColorPresetAsset item)
    {
        return _type switch
        {
            DisplayColorPreset.ParentType.Avatar => BuildController.Instance.CurrentlySelected.value.Cosmetics.Avatar.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.Eyes => BuildController.Instance.CurrentlySelected.value.Cosmetics.Eyes.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.Hair => BuildController.Instance.CurrentlySelected.value.Cosmetics.Hair.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.ApparelHeadgear => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Headgear.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.ApparelClothing => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Clothing.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.ApparelLegwear => BuildController.Instance.CurrentlySelected.value.Equipment.Outfit.Legwear.Color.Id == item.AssetObject.Guid,
            _ => default
        };
    }
}
