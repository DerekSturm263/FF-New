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

[CreateAssetMenu(menuName = "Quantum/PlayerState/ActionState/BurstState", order = Quantum.EditorDefines.AssetMenuPriorityStart + 391)]
public partial class BurstStateAsset : ActionStateAsset {
  public Quantum.BurstState Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.BurstState();
    }
    base.Reset();
  }
}

public static partial class BurstStateAssetExts {
  public static BurstStateAsset GetUnityAsset(this BurstState data) {
    return data == null ? null : UnityDB.FindAsset<BurstStateAsset>(data);
  }
}
