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

[CreateAssetMenu(menuName = "Quantum/PlayerState/DeadState", order = Quantum.EditorDefines.AssetMenuPriorityStart + 393)]
public partial class DeadStateAsset : PlayerStateAsset {
  public Quantum.DeadState Settings_DeadState;

  public override string AssetObjectPropertyPath => nameof(Settings_DeadState);
  
  public override Quantum.AssetObject AssetObject => Settings_DeadState;
  
  public override void Reset() {
    if (Settings_DeadState == null) {
      Settings_DeadState = new Quantum.DeadState();
    }
    base.Reset();
  }
}

public static partial class DeadStateAssetExts {
  public static DeadStateAsset GetUnityAsset(this DeadState data) {
    return data == null ? null : UnityDB.FindAsset<DeadStateAsset>(data);
  }
}
