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

[CreateAssetMenu(menuName = "Quantum/InfoAsset/SubEnhancer/PiercingSubEnhancer", order = Quantum.EditorDefines.AssetMenuPriorityStart + 223)]
public partial class PiercingSubEnhancerAsset : SubEnhancerAsset {
  public Quantum.PiercingSubEnhancer Settings_PiercingSubEnhancer;

  public override string AssetObjectPropertyPath => nameof(Settings_PiercingSubEnhancer);
  
  public override Quantum.AssetObject AssetObject => Settings_PiercingSubEnhancer;
  
  public override void Reset() {
    if (Settings_PiercingSubEnhancer == null) {
      Settings_PiercingSubEnhancer = new Quantum.PiercingSubEnhancer();
    }
    base.Reset();
  }
}

public static partial class PiercingSubEnhancerAssetExts {
  public static PiercingSubEnhancerAsset GetUnityAsset(this PiercingSubEnhancer data) {
    return data == null ? null : UnityDB.FindAsset<PiercingSubEnhancerAsset>(data);
  }
}