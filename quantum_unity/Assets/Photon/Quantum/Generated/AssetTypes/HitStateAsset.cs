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

[CreateAssetMenu(menuName = "Quantum/PlayerState/HitState", order = Quantum.EditorDefines.AssetMenuPriorityStart + 397)]
public partial class HitStateAsset : PlayerStateAsset {
  public Quantum.HitState Settings_HitState;

  public override string AssetObjectPropertyPath => nameof(Settings_HitState);
  
  public override Quantum.AssetObject AssetObject => Settings_HitState;
  
  public override void Reset() {
    if (Settings_HitState == null) {
      Settings_HitState = new Quantum.HitState();
    }
    base.Reset();
  }
}

public static partial class HitStateAssetExts {
  public static HitStateAsset GetUnityAsset(this HitState data) {
    return data == null ? null : UnityDB.FindAsset<HitStateAsset>(data);
  }
}
