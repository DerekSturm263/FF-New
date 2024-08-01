// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial  
// declarations in another file.
// </auto-generated>

using Quantum;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum/InfoAsset/ColorPreset", order = Quantum.EditorDefines.AssetMenuPriorityStart + 210)]
public partial class ColorPresetAsset : InfoAssetAsset {
  public Quantum.ColorPreset Settings_ColorPreset;

  public override string AssetObjectPropertyPath => nameof(Settings_ColorPreset);
  
  public override Quantum.AssetObject AssetObject => Settings_ColorPreset;
  
  public override void Reset() {
    if (Settings_ColorPreset == null) {
      Settings_ColorPreset = new Quantum.ColorPreset();
    }
    base.Reset();
  }
}

public static partial class ColorPresetAssetExts {
  public static ColorPresetAsset GetUnityAsset(this ColorPreset data) {
    return data == null ? null : UnityDB.FindAsset<ColorPresetAsset>(data);
  }
}