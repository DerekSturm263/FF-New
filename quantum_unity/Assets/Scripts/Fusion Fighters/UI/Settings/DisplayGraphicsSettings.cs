using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

public class DisplayGraphicsSettings : Display<GraphicsSettings, List<Selectable>>
{
    protected override GraphicsSettings GetValue() => SettingsController.Instance.Settings.Graphics;

    public override void UpdateDisplay(GraphicsSettings item)
    {
        (_component[0] as Toggle).isOn = item.UsePostProcessing;
        (_component[1] as Toggle).isOn = item.UseVSync;
        (_component[2] as Slider).value = item.LightCount;
        (_component[3] as Slider).value = item.ShadowDistance;
        (_component[4] as Toggle).isOn = item.RealtimeReflections;
        (_component[5] as Toggle).isOn = item.UseVFX;
        (_component[6] as Toggle).isOn = item.UseAnisotropicFiltering;
        (_component[7] as Slider).value = item.AntiAliasing;
    }
}
