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

[CreateAssetMenu(menuName = "Quantum/FrameEvent/ContinueAnimationEvent", order = Quantum.EditorDefines.AssetMenuPriorityStart + 132)]
public partial class ContinueAnimationEventAsset : FrameEventAsset {
  public Quantum.ContinueAnimationEvent Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.ContinueAnimationEvent();
    }
    base.Reset();
  }
}

public static partial class ContinueAnimationEventAssetExts {
  public static ContinueAnimationEventAsset GetUnityAsset(this ContinueAnimationEvent data) {
    return data == null ? null : UnityDB.FindAsset<ContinueAnimationEventAsset>(data);
  }
}