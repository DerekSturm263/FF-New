using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

public class DisplayAudioSettings : Display<AudioSettings, List<Selectable>>
{
    protected override AudioSettings GetValue() => (SettingsController.Instance as SettingsController).Settings.Audio;

    public override void UpdateDisplay(AudioSettings item)
    {
        (_component[0] as Slider).value = item.Volume.Master;
        (_component[1] as Slider).value = item.Volume.PlayerMaster;
        (_component[2] as Slider).value = item.Volume.PlayerSFX;
        (_component[3] as Slider).value = item.Volume.PlayerVoiceLines;
        (_component[4] as Slider).value = item.Volume.EnvironmentMaster;
        (_component[5] as Slider).value = item.Volume.EnvironmentAmbience;
        (_component[6] as Slider).value = item.Volume.EnvironmentSFX;
        (_component[7] as Slider).value = item.Volume.UIMaster;
        (_component[8] as Slider).value = item.Volume.UISFX;
        (_component[9] as Slider).value = item.Volume.UIMusic;
    }
}
