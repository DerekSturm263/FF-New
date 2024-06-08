using Extensions.Components.UI;

public class DisplaySettings : Display<Settings, TMPro.TMP_Dropdown>
{
    protected override Settings GetValue() => SettingsController.Instance.Settings;

    public override void UpdateDisplay(Settings item)
    {
        _component.value = item.GraphicsQualityPreset;
    }
}
