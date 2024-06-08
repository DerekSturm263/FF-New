using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

public class DisplayAudioSettings : Display<AudioSettings, List<Selectable>>
{
    protected override AudioSettings GetValue() => SettingsController.Instance.Settings.Audio;

    public override void UpdateDisplay(AudioSettings item)
    {
        (_component[0] as Slider).value = item.MasterVolume;
        (_component[1] as Slider).value = item.PlayerMasterVolume;
        (_component[2] as Slider).value = item.PlayerSFXVolume;
        (_component[3] as Slider).value = item.PlayerVoiceLineVolume;
        (_component[4] as Slider).value = item.EnvironmentMasterVolume;
        (_component[5] as Slider).value = item.EnvironmentAmbienceVolume;
        (_component[6] as Slider).value = item.EnvironmentSFXVolume;
        (_component[7] as Slider).value = item.UIMasterVolume;
        (_component[8] as Slider).value = item.UISFXVolume;
        (_component[9] as Slider).value = item.UIMusicVolume;
    }
}
