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

[CreateAssetMenu(menuName = "Quantum/InfoAsset/ApparelAsset", order = Quantum.EditorDefines.AssetMenuPriorityStart + 208)]
public partial class ApparelAssetAsset : InfoAssetAsset {
  public Quantum.ApparelAsset Settings_ApparelAsset;

  public override string AssetObjectPropertyPath => nameof(Settings_ApparelAsset);
  
  public override Quantum.AssetObject AssetObject => Settings_ApparelAsset;
  
  public override void Reset() {
    if (Settings_ApparelAsset == null) {
      Settings_ApparelAsset = new Quantum.ApparelAsset();
    }
    base.Reset();
  }
}

public static partial class ApparelAssetAssetExts {
  public static ApparelAssetAsset GetUnityAsset(this ApparelAsset data) {
    return data == null ? null : UnityDB.FindAsset<ApparelAssetAsset>(data);
  }
}