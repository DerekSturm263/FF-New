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

[CreateAssetMenu(menuName = "Quantum/Badge", order = Quantum.EditorDefines.AssetMenuPriorityStart + 26)]
public partial class BadgeAsset : AssetBase {
  public Quantum.Badge Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.Badge();
    }
    base.Reset();
  }
}

public static partial class BadgeAssetExts {
  public static BadgeAsset GetUnityAsset(this Badge data) {
    return data == null ? null : UnityDB.FindAsset<BadgeAsset>(data);
  }
}
