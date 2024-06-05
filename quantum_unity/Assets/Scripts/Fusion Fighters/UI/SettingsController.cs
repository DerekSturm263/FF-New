using UnityEngine;
using Extensions.Components.Miscellaneous;
using UnityEngine.Audio;
using GameResources.Camera;

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

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Shutdown()
    {
        base.Shutdown();
    }

    public void SetGameplayLanguageText()
    {

    }

    public void SetGameplayLanguageVoice()
    {

    }

    public void SetGraphicsQualityPreset(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetGraphicsQualityPostProcessing()
    {
        //CameraController.Instance.Cam.
    }

    public void SetGraphicsQualityVSync()
    {

    }

    public void SetGraphicsQualityLights()
    {

    }

    public void SetGraphicsQualityShadows()
    {

    }

    public void SetGraphicsQualityHDR()
    {

    }

    public void SetGraphicsQualityVFX()
    {

    }

    public void SetGraphicsQualityTextureQuality()
    {

    }

    public void SetGraphicsQualityAntiAliasing()
    {

    }

    public void SetGraphicsColorPreset()
    {

    }

    public void SetGraphicsColorRed()
    {

    }

    public void SetGraphicsColorGreen()
    {

    }

    public void SetGraphicsColorBlue()
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
