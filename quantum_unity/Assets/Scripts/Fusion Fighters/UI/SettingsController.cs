using UnityEngine;
using Extensions.Components.Miscellaneous;
using UnityEngine.Audio;
using GameResources.Camera;
using GameResources;

public class SettingsController : Controller<SettingsController>
{
    [SerializeField] private AudioMixerGroup _master;
    [SerializeField] private AudioMixerGroup _playerMaster;
    [SerializeField] private AudioMixerGroup _playerSFX;
    [SerializeField] private AudioMixerGroup _playerVoiceLines;
    [SerializeField] private AudioMixerGroup _environmentMaster;
    [SerializeField] private AudioMixerGroup _environmentAmbience;
    [SerializeField] private AudioMixerGroup _environmentSFX;
    [SerializeField] private AudioMixerGroup _uiMaster;
    [SerializeField] private AudioMixerGroup _uiMusic;
    [SerializeField] private AudioMixerGroup _uiSFX;

    [SerializeField] private UnityEngine.Rendering.VolumeProfile _volume;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Shutdown()
    {
        base.Shutdown();
    }

    public void SetGameplayLanguageText(int index)
    {

    }

    public void SetGameplayLanguageVoice(int index)
    {

    }

    public void SetGraphicsQualityPreset(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetGraphicsQualityPostProcessing(bool isEnabled)
    {
        if (isEnabled)
            CameraController.Instance.SetVolume(_volume);
        else
            CameraController.Instance.SetVolume(null);
    }

    public void SetGraphicsQualityVSync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
    }

    public void SetGraphicsQualityLights(float value)
    {
        QualitySettings.pixelLightCount = (int)value;
    }

    public void SetGraphicsQualityShadows(float value)
    {
        QualitySettings.shadowDistance = value;
    }

    public void SetGraphicsQualityRealtimeReflections(bool isEnabled)
    {
        QualitySettings.realtimeReflectionProbes = isEnabled;
    }

    public void SetGraphicsQualityVFX(bool isEnabled)
    {
        VFXController.Instance.SetIsEnabled(isEnabled);
    }

    public void SetGraphicsQualityAnisotropicFiltering(bool isEnabled)
    {
        QualitySettings.anisotropicFiltering = isEnabled ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
    }

    public void SetGraphicsQualityAntiAliasing(float value)
    {
        QualitySettings.antiAliasing = (int)(value * value);
    }

    public void SetGraphicsColorPreset(int index)
    {

    }

    public void SetGraphicsColorRed(Color red)
    {

    }

    public void SetGraphicsColorGreen(Color green)
    {

    }

    public void SetGraphicsColorBlue(Color blue)
    {

    }

    public void SetAudioVolumeMaster(float volume)
    {
        if (volume <= 0.01f)
            _master.audioMixer.SetFloat("Volume", -80);
        else
            _master.audioMixer.SetFloat("Volume", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumePlayerMaster(float volume)
    {
        if (volume <= 0.01f)
            _playerMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _playerMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumePlayerSFX(float volume)
    {
        if (volume <= 0.01f)
            _playerSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _playerSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumePlayerVoiceLines(float volume)
    {
        if (volume <= 0.01f)
            _playerVoiceLines.audioMixer.SetFloat("VolumeVoiceLines", -80);
        else
            _playerVoiceLines.audioMixer.SetFloat("VolumeVoiceLines", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumeEnvironmentMaster(float volume)
    {
        if (volume <= 0.01f)
            _environmentMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _environmentMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumeEnvironmentAmbience(float volume)
    {
        if (volume <= 0.01f)
            _environmentAmbience.audioMixer.SetFloat("VolumeAmbience", -80);
        else
            _environmentAmbience.audioMixer.SetFloat("VolumeAmbience", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumeEnvironmentSFX(float volume)
    {
        if (volume <= 0.01f)
            _environmentSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _environmentSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumeUIMaster(float volume)
    {
        if (volume <= 0.01f)
            _uiMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _uiMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumeUISFX(float volume)
    {
        if (volume <= 0.01f)
            _uiSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _uiSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);
    }

    public void SetAudioVolumeUIMusic(float volume)
    {
        if (volume <= 0.01f)
            _uiMusic.audioMixer.SetFloat("VolumeMusic", -80);
        else
            _uiMusic.audioMixer.SetFloat("VolumeMusic", Mathf.Log(volume) * 20);
    }
}
