using Extensions.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresetPopulator : PopulateAsset<ColorPresetAsset>
{
    [SerializeField] private DisplayColorPreset.ParentType _type;

    protected override string FilePath() => "DB/Assets/Build/Color Presets";

    protected override Dictionary<string, Comparison<ColorPresetAsset>> GetAllSortModes()
    {
        Dictionary<string, Comparison<ColorPresetAsset>> derivedModes = new()
        {
            ["Brightness"] = (lhs, rhs) =>
                lhs.Settings_ColorPreset.Color.R.CompareTo(rhs.Settings_ColorPreset.Color.R) +
                lhs.Settings_ColorPreset.Color.G.CompareTo(rhs.Settings_ColorPreset.Color.G) +
                lhs.Settings_ColorPreset.Color.B.CompareTo(rhs.Settings_ColorPreset.Color.B)
        };

        return derivedModes.Concat(base.GetAllSortModes()).ToDictionary(item => item.Key, item => item.Value);
    }

    protected override void Decorate(GameObject buttonObj, ColorPresetAsset item)
    {
        base.Decorate(buttonObj, item);

        buttonObj.FindChildWithTag("Icon", false).GetComponent<Image>().color = item.Settings_ColorPreset.Color.ToColor();
    }

    protected override bool IsEquipped(ColorPresetAsset item)
    {
        return _type switch
        {
            DisplayColorPreset.ParentType.Avatar => BuildController.Instance.CurrentBuild.value.Frame.Avatar.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.Eyes => BuildController.Instance.CurrentBuild.value.Frame.Eyes.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.Hair => BuildController.Instance.CurrentBuild.value.Frame.Hair.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.ApparelHeadgear => BuildController.Instance.CurrentBuild.value.Outfit.Headgear.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.ApparelClothing => BuildController.Instance.CurrentBuild.value.Outfit.Clothing.Color.Id == item.AssetObject.Guid,
            DisplayColorPreset.ParentType.ApparelLegwear => BuildController.Instance.CurrentBuild.value.Outfit.Legwear.Color.Id == item.AssetObject.Guid,
            _ => default
        };
    }
}
