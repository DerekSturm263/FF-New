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

[CreateAssetMenu(menuName = "Quantum/ColorPalette", order = Quantum.EditorDefines.AssetMenuPriorityStart + 52)]
public partial class ColorPaletteAsset : AssetBase {
  public Quantum.ColorPalette Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.ColorPalette();
    }
    base.Reset();
  }
}

public static partial class ColorPaletteAssetExts {
  public static ColorPaletteAsset GetUnityAsset(this ColorPalette data) {
    return data == null ? null : UnityDB.FindAsset<ColorPaletteAsset>(data);
  }
}
