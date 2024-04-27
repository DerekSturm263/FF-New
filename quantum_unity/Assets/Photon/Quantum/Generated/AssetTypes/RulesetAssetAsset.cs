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

[CreateAssetMenu(menuName = "Quantum/RulesetAsset", order = Quantum.EditorDefines.AssetMenuPriorityStart + 442)]
public partial class RulesetAssetAsset : AssetBase {
  public Quantum.RulesetAsset Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.RulesetAsset();
    }
    base.Reset();
  }
}

public static partial class RulesetAssetAssetExts {
  public static RulesetAssetAsset GetUnityAsset(this RulesetAsset data) {
    return data == null ? null : UnityDB.FindAsset<RulesetAssetAsset>(data);
  }
}
