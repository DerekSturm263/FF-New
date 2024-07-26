using UnityEngine;
using UnityEngine.Audio;
using GameResources.Camera;
using GameResources;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using Quantum;
using Photon.Deterministic;

public class SettingsController : SpawnableController<Settings>
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

    [SerializeField] private SettingsAsset _default;

    private Settings _settings;
    public Settings Settings => _settings;

    [System.NonSerialized] private bool _isInitialized = false;

    public void Spawn()
    {
        Spawn(default);
    }

    public override void Initialize()
    {
        base.Initialize();

        if (!_isInitialized)
        {
            if (FusionFighters.Serializer.TryLoadAs($"{Application.persistentDataPath}/SaveData/Misc/Settings.json", $"{Application.persistentDataPath}/SaveData/Misc", out Settings settings))
                _settings = settings.DeepCopy();
            else
                _settings = _default.Settings.DeepCopy();

            Application.quitting += Shutdown;
            _isInitialized = true;
        }

        SetGraphicsPreferencesFullscreen(_settings.Graphics.Preferences.UseFullscreen);

        SetGraphicsQualityPresetNoSet(_settings.Graphics.QualityPreset);
        SetGraphicsQualityPostProcessing(_settings.Graphics.Quality.UsePostProcessing);
        SetGraphicsQualityVSync(_settings.Graphics.Quality.UseVSync);
        SetGraphicsQualityLights(_settings.Graphics.Quality.LightCount);
        SetGraphicsQualityShadows(_settings.Graphics.Quality.ShadowDistance);
        SetGraphicsQualityRealtimeReflections(_settings.Graphics.Quality.RealtimeReflections);
        SetGraphicsQualityVFX(_settings.Graphics.Quality.UseVFX);
        SetGraphicsQualityAnisotropicFiltering(_settings.Graphics.Quality.UseAnisotropicFiltering);
        SetGraphicsQualityAntiAliasing(_settings.Graphics.Quality.AntiAliasing);

        SetAudioVolumeMaster(_settings.Audio.Volume.Master);
        SetAudioVolumePlayerMaster(_settings.Audio.Volume.PlayerMaster);
        SetAudioVolumePlayerSFX(_settings.Audio.Volume.PlayerSFX);
        SetAudioVolumePlayerVoiceLines(_settings.Audio.Volume.PlayerVoiceLines);
        SetAudioVolumeEnvironmentMaster(_settings.Audio.Volume.EnvironmentMaster);
        SetAudioVolumeEnvironmentAmbience(_settings.Audio.Volume.EnvironmentAmbience);
        SetAudioVolumeEnvironmentSFX(_settings.Audio.Volume.EnvironmentSFX);
        SetAudioVolumeUIMaster(_settings.Audio.Volume.UIMaster);
        SetAudioVolumeUISFX(_settings.Audio.Volume.UISFX);
        SetAudioVolumeUIMusic(_settings.Audio.Volume.UIMusic);
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _isInitialized = false;

        FusionFighters.Serializer.Save(_settings, "Settings", $"{Application.persistentDataPath}/SaveData/Misc");
        
        base.Shutdown();
    }

    public void SetGameplayLanguageText(int index)
    {

    }

    public void SetGameplayLanguageVoice(int index)
    {

    }

    public void SetGraphicsPreferencesFullscreen(bool isEnabled)
    {
        Screen.fullScreenMode = isEnabled ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;

        _settings.Graphics.Preferences.UseFullscreen = isEnabled;
    }

    public void SetGraphicsQualityPresetNoSet(int index)
    {
        QualitySettings.SetQualityLevel(index);

        _settings.Graphics.QualityPreset = index;
    }

    public void SetGraphicsQualityPreset(int index)
    {
        QualitySettings.SetQualityLevel(index);

        _settings.Graphics.QualityPreset = index;

        switch (_settings.Graphics.QualityPreset)
        {
            case 0:
                _settings.Graphics.Quality = GraphicsQualitySettings.Max;
                break;

            case 1:
                _settings.Graphics.Quality = GraphicsQualitySettings.High;
                break;

            case 2:
                _settings.Graphics.Quality = GraphicsQualitySettings.Medium;
                break;

            case 3:
                _settings.Graphics.Quality = GraphicsQualitySettings.Low;
                break;

            case 4:
                _settings.Graphics.Quality = GraphicsQualitySettings.Min;
                break;
        }
    }

    public void SetGraphicsQualityPostProcessing(bool isEnabled)
    {
        if (CameraController.Instance)
            CameraController.Instance.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = isEnabled;

        _settings.Graphics.Quality.UsePostProcessing = isEnabled;
    }

    public void SetGraphicsQualityVSync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;

        _settings.Graphics.Quality.UseVSync = isEnabled;
    }

    public void SetGraphicsQualityLights(float value)
    {
        UniversalRenderPipeline.asset.maxAdditionalLightsCount = (int)value;

        _settings.Graphics.Quality.LightCount = (int)value;
    }

    public void SetGraphicsQualityShadows(float value)
    {
        UniversalRenderPipeline.asset.shadowDistance = value;

        _settings.Graphics.Quality.ShadowDistance = value;
    }

    public void SetGraphicsQualityRealtimeReflections(bool isEnabled)
    {
        QualitySettings.realtimeReflectionProbes = isEnabled;

        _settings.Graphics.Quality.RealtimeReflections = isEnabled;
    }

    public void SetGraphicsQualityVFX(bool isEnabled)
    {
        VFXController.Instance.SetIsEnabled(isEnabled);

        VisualEffect vfx = FindFirstObjectByType<VisualEffect>();
        if (vfx)
            vfx.enabled = isEnabled;

        _settings.Graphics.Quality.UseVFX = isEnabled;
    }

    public void SetGraphicsQualityAnisotropicFiltering(bool isEnabled)
    {
        QualitySettings.anisotropicFiltering = isEnabled ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;

        _settings.Graphics.Quality.UseAnisotropicFiltering = isEnabled;
    }

    public void SetGraphicsQualityAntiAliasing(float value)
    {
        if (value == -1)
            QualitySettings.antiAliasing = 0;
        else
            QualitySettings.antiAliasing = (int)Mathf.Pow(2, value);

        _settings.Graphics.Quality.AntiAliasing = (int)value;
    }

    public void SetGraphicsColorPreset(int index)
    {
        _settings.Graphics.ColorsPreset = index;

        switch (_settings.Graphics.ColorsPreset)
        {
            case 0:
                _settings.Graphics.Colors = GraphicsColorSettings.Default;
                break;

            case 1:
                _settings.Graphics.Colors = GraphicsColorSettings.ColorBlindRedGreen;
                break;

            case 2:
                _settings.Graphics.Colors = GraphicsColorSettings.ColorBlindBlueYellow;
                break;
        }
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

        _settings.Audio.Volume.Master = volume;
    }

    public void SetAudioVolumePlayerMaster(float volume)
    {
        if (volume <= 0.01f)
            _playerMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _playerMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.PlayerMaster = volume;
    }

    public void SetAudioVolumePlayerSFX(float volume)
    {
        if (volume <= 0.01f)
            _playerSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _playerSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.PlayerSFX = volume;
    }

    public void SetAudioVolumePlayerVoiceLines(float volume)
    {
        if (volume <= 0.01f)
            _playerVoiceLines.audioMixer.SetFloat("VolumeVoiceLines", -80);
        else
            _playerVoiceLines.audioMixer.SetFloat("VolumeVoiceLines", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.PlayerVoiceLines = volume;
    }

    public void SetAudioVolumeEnvironmentMaster(float volume)
    {
        if (volume <= 0.01f)
            _environmentMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _environmentMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.EnvironmentMaster = volume;
    }

    public void SetAudioVolumeEnvironmentAmbience(float volume)
    {
        if (volume <= 0.01f)
            _environmentAmbience.audioMixer.SetFloat("VolumeAmbience", -80);
        else
            _environmentAmbience.audioMixer.SetFloat("VolumeAmbience", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.EnvironmentAmbience = volume;
    }

    public void SetAudioVolumeEnvironmentSFX(float volume)
    {
        if (volume <= 0.01f)
            _environmentSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _environmentSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.EnvironmentSFX = volume;
    }

    public void SetAudioVolumeUIMaster(float volume)
    {
        if (volume <= 0.01f)
            _uiMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _uiMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.UIMaster = volume;
    }

    public void SetAudioVolumeUISFX(float volume)
    {
        if (volume <= 0.01f)
            _uiSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _uiSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.UISFX = volume;
    }

    public void SetAudioVolumeUIMusic(float volume)
    {
        if (volume <= 0.01f)
            _uiMusic.audioMixer.SetFloat("VolumeMusic", -80);
        else
            _uiMusic.audioMixer.SetFloat("VolumeMusic", Mathf.Log(volume) * 20);

        _settings.Audio.Volume.UIMusic = volume;
    }

    public void Quit()
    {
        QuantumRunner.Default.Shutdown();
    }

    public void SetTimeScale(int scaleIndex)
    {
        CommandSetTimeScale setTimeScale = new()
        {
            scale = scaleIndex switch {
                0 => FP._0,
                1 => FP._0_50,
                2 => FP._1,
                _ => FP._1,
            }
        };

        Time.timeScale = setTimeScale.scale.AsFloat;
        QuantumRunner.Default.Game.SendCommand(setTimeScale);
    }

    public void EraseAllSaveData()
    {
        System.IO.Directory.Delete($"{Application.persistentDataPath}/SaveData", true);
    }
}
