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

[CreateAssetMenu(menuName = "Quantum/FrameEvent/ModifyHurtboxesEvent", order = Quantum.EditorDefines.AssetMenuPriorityStart + 142)]
public partial class ModifyHurtboxesEventAsset : FrameEventAsset {
  public Quantum.ModifyHurtboxesEvent Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.ModifyHurtboxesEvent();
    }
    base.Reset();
  }
}

public static partial class ModifyHurtboxesEventAssetExts {
  public static ModifyHurtboxesEventAsset GetUnityAsset(this ModifyHurtboxesEvent data) {
    return data == null ? null : UnityDB.FindAsset<ModifyHurtboxesEventAsset>(data);
  }
}
