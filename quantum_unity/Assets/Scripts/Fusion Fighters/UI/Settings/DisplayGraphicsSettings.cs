using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

public class DisplayGraphicsSettings : Display<GraphicsSettings, List<Selectable>>
{
    protected override GraphicsSettings GetValue() => (SettingsController.Instance as SettingsController).Settings.Graphics;

    public override void UpdateDisplay(GraphicsSettings item)
    {
        (_component[0] as Toggle).isOn = item.Quality.UsePostProcessing;
        (_component[1] as Toggle).isOn = item.Quality.UseVSync;
        (_component[2] as Slider).value = item.Quality.LightCount;
        (_component[3] as Slider).value = item.Quality.ShadowDistance;
        (_component[4] as Toggle).isOn = item.Quality.RealtimeReflections;
        (_component[5] as Toggle).isOn = item.Quality.UseVFX;
        (_component[6] as Toggle).isOn = item.Quality.UseAnisotropicFiltering;
        (_component[7] as Slider).value = item.Quality.AntiAliasing;
        (_component[8] as Toggle).isOn = item.Preferences.UseFullscreen;
    }
}
