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

[CreateAssetMenu(menuName = "Quantum/PlayerState/ActionState/InteractState", order = Quantum.EditorDefines.AssetMenuPriorityStart + 398)]
public partial class InteractStateAsset : ActionStateAsset {
  public Quantum.InteractState Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.InteractState();
    }
    base.Reset();
  }
}

public static partial class InteractStateAssetExts {
  public static InteractStateAsset GetUnityAsset(this InteractState data) {
    return data == null ? null : UnityDB.FindAsset<InteractStateAsset>(data);
  }
}
