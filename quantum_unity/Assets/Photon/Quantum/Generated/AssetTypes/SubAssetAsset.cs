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

[CreateAssetMenu(menuName = "Quantum/InfoAsset/SubAsset", order = Quantum.EditorDefines.AssetMenuPriorityStart + 226)]
public partial class SubAssetAsset : InfoAssetAsset {
  public Quantum.SubAsset Settings_SubAsset;

  public override string AssetObjectPropertyPath => nameof(Settings_SubAsset);
  
  public override Quantum.AssetObject AssetObject => Settings_SubAsset;
  
  public override void Reset() {
    if (Settings_SubAsset == null) {
      Settings_SubAsset = new Quantum.SubAsset();
    }
    base.Reset();
  }
}

public static partial class SubAssetAssetExts {
  public static SubAssetAsset GetUnityAsset(this SubAsset data) {
    return data == null ? null : UnityDB.FindAsset<SubAssetAsset>(data);
  }
}
