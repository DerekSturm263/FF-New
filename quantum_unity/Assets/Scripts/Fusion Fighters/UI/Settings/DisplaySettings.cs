using Extensions.Components.UI;

public class DisplaySettings : Display<Settings, TMPro.TMP_Dropdown>
{
    protected override Settings GetValue() => (SettingsController.Instance as SettingsController).Settings;

    public override void UpdateDisplay(Settings item)
    {
        _component.value = item.Graphics.QualityPreset;
    }
}
