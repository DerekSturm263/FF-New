[System.Serializable]
public struct Settings
{
    public static Settings Default = new()
    {
        GraphicsQualityPreset = 1,
        Graphics = GraphicsSettings.High,
        Audio = AudioSettings.Default
    };

    public int GraphicsQualityPreset;
    public GraphicsSettings Graphics;
    public AudioSettings Audio;
}

[System.Serializable]
public struct GraphicsSettings
{
    public static GraphicsSettings Max = new()
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

    public static GraphicsSettings High = new()
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

    public static GraphicsSettings Medium = new()
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

    public static GraphicsSettings Low = new()
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

    public static GraphicsSettings Min = new()
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
public struct AudioSettings
{
    public static AudioSettings Default = new()
    {
        MasterVolume = 1,
        PlayerMasterVolume = 1,
        PlayerSFXVolume = 1,
        PlayerVoiceLineVolume = 1,
        EnvironmentMasterVolume = 1,
        EnvironmentAmbienceVolume = 1,
        EnvironmentSFXVolume = 1,
        UIMasterVolume = 1,
        UISFXVolume = 1,
        UIMusicVolume = 1
    };

    public float MasterVolume;
    public float PlayerMasterVolume;
    public float PlayerSFXVolume;
    public float PlayerVoiceLineVolume;
    public float EnvironmentMasterVolume;
    public float EnvironmentAmbienceVolume;
    public float EnvironmentSFXVolume;
    public float UIMasterVolume;
    public float UISFXVolume;
    public float UIMusicVolume;
}
