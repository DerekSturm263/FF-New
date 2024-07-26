using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Settings
{
    public List<string> VisitedScenes;

    public GameplaySettings Gameplay;
    public GraphicsSettings Graphics;
    public AudioSettings Audio;

    public Settings DeepCopy()
    {
        return new()
        {
            VisitedScenes = new(VisitedScenes),
            Gameplay = Gameplay,
            Graphics = Graphics,
            Audio = Audio
        };
    }
}

[System.Serializable]
public struct GameplaySettings
{
    public static GameplaySettings Default = new() { };
}

[System.Serializable]
public struct GraphicsSettings
{
    public static GraphicsSettings Default = new()
    {
        Preferences = GraphicsPreferenceSettings.Default,
        QualityPreset = 1,
        Quality = GraphicsQualitySettings.High,
        ColorsPreset = 0,
        Colors = GraphicsColorSettings.Default
    };

    public GraphicsPreferenceSettings Preferences;
    public int QualityPreset;
    public GraphicsQualitySettings Quality;
    public int ColorsPreset;
    public GraphicsColorSettings Colors;
}

[System.Serializable]
public struct GraphicsPreferenceSettings
{
    public static GraphicsPreferenceSettings Default = new ()
    {
        UseFullscreen = true
    };

    public bool UseFullscreen;
}

[System.Serializable]
public struct GraphicsQualitySettings
{
    public static GraphicsQualitySettings Max = new()
    {
        UsePostProcessing = true,
        UseVSync = true,
        LightCount = 8,
        ShadowDistance = 50,
        RealtimeReflections = true,
        UseVFX = true,
        UseAnisotropicFiltering = true,
        AntiAliasing = 3
    };

    public static GraphicsQualitySettings High = new()
    {
        UsePostProcessing = true,
        UseVSync = false,
        LightCount = 8,
        ShadowDistance = 50,
        RealtimeReflections = true,
        UseVFX = true,
        UseAnisotropicFiltering = true,
        AntiAliasing = 2
    };

    public static GraphicsQualitySettings Medium = new()
    {
        UsePostProcessing = true,
        UseVSync = true,
        LightCount = 4,
        ShadowDistance = 25,
        RealtimeReflections = false,
        UseVFX = true,
        UseAnisotropicFiltering = true,
        AntiAliasing = 1
    };

    public static GraphicsQualitySettings Low = new()
    {
        UsePostProcessing = false,
        UseVSync = false,
        LightCount = 2,
        ShadowDistance = 10,
        RealtimeReflections = false,
        UseVFX = false,
        UseAnisotropicFiltering = false,
        AntiAliasing = 0
    };

    public static GraphicsQualitySettings Min = new()
    {
        UsePostProcessing = false,
        UseVSync = false,
        LightCount = 0,
        ShadowDistance = 0,
        RealtimeReflections = false,
        UseVFX = false,
        UseAnisotropicFiltering = false,
        AntiAliasing = -1
    };

    public bool UsePostProcessing;
    public bool UseVSync;
    public int LightCount;
    public float ShadowDistance;
    public bool RealtimeReflections;
    public bool UseVFX;
    public bool UseAnisotropicFiltering;
    public int AntiAliasing;
}

[System.Serializable]
public struct GraphicsColorSettings
{
    public static GraphicsColorSettings Default = new() { };
    public static GraphicsColorSettings ColorBlindRedGreen = new() { };
    public static GraphicsColorSettings ColorBlindBlueYellow = new() { };

    public float RedMapping;
    public float GreenMapping;
    public float BlueMapping;
    public float Srength;
}

[System.Serializable]
public struct AudioSettings
{
    public static AudioSettings Default = new()
    {
        Volume = AudioVolumeSettings.Default,
        Subtitles = AudioSubtitleSettings.Default
    };

    public AudioVolumeSettings Volume;
    public AudioSubtitleSettings Subtitles;
}

[System.Serializable]
public struct AudioVolumeSettings
{
    public static AudioVolumeSettings Default = new()
    {
        Master = 1,
        PlayerMaster = 1,
        PlayerSFX = 1,
        PlayerVoiceLines = 1,
        EnvironmentMaster = 1,
        EnvironmentAmbience = 1,
        EnvironmentSFX = 1,
        UIMaster = 1,
        UISFX = 1,
        UIMusic = 1
    };

    public float Master;
    public float PlayerMaster;
    public float PlayerSFX;
    public float PlayerVoiceLines;
    public float EnvironmentMaster;
    public float EnvironmentAmbience;
    public float EnvironmentSFX;
    public float UIMaster;
    public float UISFX;
    public float UIMusic;
}

[System.Serializable]
public struct AudioSubtitleSettings
{
    public enum Language
    {
        English,
        Mandarin,
        Hindi,
        Spanish,
        French,
        Arabic,
        Bengali,
        Portuguese,
        Russian,
        Urdu
    }

    public static AudioSubtitleSettings Default = new() { };

    public Color TextColor;
    public Color BackgroundColor;
    public Language WrittenLanguage;
}
