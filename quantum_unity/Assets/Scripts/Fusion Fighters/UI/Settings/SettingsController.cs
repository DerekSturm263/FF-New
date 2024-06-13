using UnityEngine;
using Extensions.Components.Miscellaneous;
using UnityEngine.Audio;
using GameResources.Camera;
using GameResources;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class SettingsController : Controller<SettingsController>
{
    [SerializeField] private GameObject _popup;
    private GameObject _popupInstance;

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

    private Settings _settings;
    public Settings Settings => _settings;

    [System.NonSerialized] private bool _isInitialized = false;

    private GameObject _oldSelected;
    private List<(Selectable, bool)> _allSelectables;

    private IEnumerable<Extensions.Components.Input.InputEvent> _inputEvents;
    private Dictionary<Extensions.Components.Input.InputEvent, bool> _wasEnabled;

    public override void Initialize()
    {
        base.Initialize();

        if (Serializer.TryLoadAs($"{Application.persistentDataPath}/ApplicationSettings.json", $"{Application.persistentDataPath}", out Settings settings))
            _settings = settings;
        else
            _settings = Settings.Default;

        SetGraphicsQualityPresetNoSet(_settings.GraphicsQualityPreset);
        SetGraphicsQualityPostProcessing(_settings.Graphics.UsePostProcessing);
        SetGraphicsQualityVSync(_settings.Graphics.UseVSync);
        SetGraphicsQualityLights(_settings.Graphics.LightCount);
        SetGraphicsQualityShadows(_settings.Graphics.ShadowDistance);
        SetGraphicsQualityRealtimeReflections(_settings.Graphics.RealtimeReflections);
        SetGraphicsQualityVFX(_settings.Graphics.UseVFX);
        SetGraphicsQualityAnisotropicFiltering(_settings.Graphics.UseAnisotropicFiltering);
        SetGraphicsQualityAntiAliasing(_settings.Graphics.AntiAliasing);

        SetAudioVolumeMaster(_settings.Audio.MasterVolume);
        SetAudioVolumePlayerMaster(_settings.Audio.PlayerMasterVolume);
        SetAudioVolumePlayerSFX(_settings.Audio.PlayerSFXVolume);
        SetAudioVolumePlayerVoiceLines(_settings.Audio.PlayerVoiceLineVolume);
        SetAudioVolumeEnvironmentMaster(_settings.Audio.EnvironmentMasterVolume);
        SetAudioVolumeEnvironmentAmbience(_settings.Audio.EnvironmentAmbienceVolume);
        SetAudioVolumeEnvironmentSFX(_settings.Audio.EnvironmentSFXVolume);
        SetAudioVolumeUIMaster(_settings.Audio.UIMasterVolume);
        SetAudioVolumeUISFX(_settings.Audio.UISFXVolume);
        SetAudioVolumeUIMusic(_settings.Audio.UIMusicVolume);

        if (!_isInitialized)
        {
            Application.quitting += Shutdown;
            _isInitialized = true;
        }
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _isInitialized = false;

        Serializer.Save(_settings, "ApplicationSettings", $"{Application.persistentDataPath}");
        
        base.Shutdown();
    }

    public void Spawn()
    {
        RememberOldSelected();

        _allSelectables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select<Selectable, (Selectable, bool)>(item => new(item, item.interactable)).ToList();
        foreach (var selectable in _allSelectables)
        {
            selectable.Item1.interactable = false;
        }

        _inputEvents = FindObjectsByType<Extensions.Components.Input.InputEvent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(item => item.gameObject != gameObject);
        _wasEnabled = new();

        foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
        {
            _wasEnabled.Add(inputEvent, inputEvent.enabled);
            inputEvent.enabled = false;
        }

        _popupInstance = Instantiate(_popup, GameObject.FindWithTag("Popup Canvas").transform);
    }

    public void Despawn()
    {
        foreach (var selectable in _allSelectables)
        {
            selectable.Item1.interactable = selectable.Item2;
        }

        SetOldSelected();

        foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
        {
            inputEvent.enabled = _wasEnabled[inputEvent];
        }

        Destroy(_popupInstance);
    }

    public void RememberOldSelected()
    {
        if (EventSystem.current)
            _oldSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void SetOldSelected()
    {
        if (EventSystem.current && _oldSelected)
            EventSystem.current.SetSelectedGameObject(_oldSelected);
    }

    public void SetGameplayLanguageText(int index)
    {

    }

    public void SetGameplayLanguageVoice(int index)
    {

    }

    public void SetGraphicsQualityPresetNoSet(int index)
    {
        QualitySettings.SetQualityLevel(index);

        _settings.GraphicsQualityPreset = index;
    }

    public void SetGraphicsQualityPreset(int index)
    {
        QualitySettings.SetQualityLevel(index);

        _settings.GraphicsQualityPreset = index;
        switch (_settings.GraphicsQualityPreset)
        {
            case 0:
                _settings.Graphics = GraphicsSettings.Max;
                break;

            case 1:
                _settings.Graphics = GraphicsSettings.High;
                break;

            case 2:
                _settings.Graphics = GraphicsSettings.Medium;
                break;

            case 3:
                _settings.Graphics = GraphicsSettings.Low;
                break;

            case 4:
                _settings.Graphics = GraphicsSettings.Min;
                break;
        }
    }

    public void SetGraphicsQualityPostProcessing(bool isEnabled)
    {
        if (CameraController.Instance)
            CameraController.Instance.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = isEnabled;

        _settings.Graphics.UsePostProcessing = isEnabled;
    }

    public void SetGraphicsQualityVSync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;

        _settings.Graphics.UseVSync = isEnabled;
    }

    public void SetGraphicsQualityLights(float value)
    {
        QualitySettings.pixelLightCount = (int)value;

        _settings.Graphics.LightCount = (int)value;
    }

    public void SetGraphicsQualityShadows(float value)
    {
        QualitySettings.shadowDistance = value;

        _settings.Graphics.ShadowDistance = value;
    }

    public void SetGraphicsQualityRealtimeReflections(bool isEnabled)
    {
        QualitySettings.realtimeReflectionProbes = isEnabled;

        _settings.Graphics.RealtimeReflections = isEnabled;
    }

    public void SetGraphicsQualityVFX(bool isEnabled)
    {
        VFXController.Instance.SetIsEnabled(isEnabled);

        VisualEffect vfx = FindFirstObjectByType<VisualEffect>();
        if (vfx)
            vfx.enabled = isEnabled;

        _settings.Graphics.UseVFX = isEnabled;
    }

    public void SetGraphicsQualityAnisotropicFiltering(bool isEnabled)
    {
        QualitySettings.anisotropicFiltering = isEnabled ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;

        _settings.Graphics.UseAnisotropicFiltering = isEnabled;
    }

    public void SetGraphicsQualityAntiAliasing(float value)
    {
        if (value == -1)
            QualitySettings.antiAliasing = 0;
        else
            QualitySettings.antiAliasing = (int)Mathf.Pow(2, value);

        _settings.Graphics.AntiAliasing = (int)value;
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

        _settings.Audio.MasterVolume = volume;
    }

    public void SetAudioVolumePlayerMaster(float volume)
    {
        if (volume <= 0.01f)
            _playerMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _playerMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);

        _settings.Audio.PlayerMasterVolume = volume;
    }

    public void SetAudioVolumePlayerSFX(float volume)
    {
        if (volume <= 0.01f)
            _playerSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _playerSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);

        _settings.Audio.PlayerSFXVolume = volume;
    }

    public void SetAudioVolumePlayerVoiceLines(float volume)
    {
        if (volume <= 0.01f)
            _playerVoiceLines.audioMixer.SetFloat("VolumeVoiceLines", -80);
        else
            _playerVoiceLines.audioMixer.SetFloat("VolumeVoiceLines", Mathf.Log(volume) * 20);

        _settings.Audio.PlayerVoiceLineVolume = volume;
    }

    public void SetAudioVolumeEnvironmentMaster(float volume)
    {
        if (volume <= 0.01f)
            _environmentMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _environmentMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);

        _settings.Audio.EnvironmentMasterVolume = volume;
    }

    public void SetAudioVolumeEnvironmentAmbience(float volume)
    {
        if (volume <= 0.01f)
            _environmentAmbience.audioMixer.SetFloat("VolumeAmbience", -80);
        else
            _environmentAmbience.audioMixer.SetFloat("VolumeAmbience", Mathf.Log(volume) * 20);

        _settings.Audio.EnvironmentAmbienceVolume = volume;
    }

    public void SetAudioVolumeEnvironmentSFX(float volume)
    {
        if (volume <= 0.01f)
            _environmentSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _environmentSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);

        _settings.Audio.EnvironmentSFXVolume = volume;
    }

    public void SetAudioVolumeUIMaster(float volume)
    {
        if (volume <= 0.01f)
            _uiMaster.audioMixer.SetFloat("VolumeMaster", -80);
        else
            _uiMaster.audioMixer.SetFloat("VolumeMaster", Mathf.Log(volume) * 20);

        _settings.Audio.UIMasterVolume = volume;
    }

    public void SetAudioVolumeUISFX(float volume)
    {
        if (volume <= 0.01f)
            _uiSFX.audioMixer.SetFloat("VolumeSFX", -80);
        else
            _uiSFX.audioMixer.SetFloat("VolumeSFX", Mathf.Log(volume) * 20);

        _settings.Audio.UISFXVolume = volume;
    }

    public void SetAudioVolumeUIMusic(float volume)
    {
        if (volume <= 0.01f)
            _uiMusic.audioMixer.SetFloat("VolumeMusic", -80);
        else
            _uiMusic.audioMixer.SetFloat("VolumeMusic", Mathf.Log(volume) * 20);

        _settings.Audio.UIMusicVolume = volume;
    }

    public void Quit()
    {
        QuantumRunner.Default.Shutdown();
    }
}
