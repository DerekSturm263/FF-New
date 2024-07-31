using Extensions.Miscellaneous;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresetPopulator : PopulateAsset<ColorPresetAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Color Presets";

    protected override bool DoSpawn(ColorPresetAsset item) => InventoryController.Instance.HasUnlockedItem(item);

    protected override Func<ColorPresetAsset, int> Sort() => (item) => -(item.Settings_ColorPreset.Color.R + item.Settings_ColorPreset.Color.G + item.Settings_ColorPreset.Color.B);

    protected override void Decorate(GameObject buttonObj, ColorPresetAsset item)
    {
        base.Decorate(buttonObj, item);

        buttonObj.FindChildWithTag("Icon").GetComponent<Image>().color = item.Settings_ColorPreset.Color.ToColor();
    }
}
