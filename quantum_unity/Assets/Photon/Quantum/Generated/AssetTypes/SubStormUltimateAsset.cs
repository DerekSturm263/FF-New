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

[CreateAssetMenu(menuName = "Quantum/Ultimate/SubStormUltimate", order = Quantum.EditorDefines.AssetMenuPriorityStart + 538)]
public partial class SubStormUltimateAsset : UltimateAsset {
  public Quantum.SubStormUltimate Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.SubStormUltimate();
    }
    base.Reset();
  }
}

public static partial class SubStormUltimateAssetExts {
  public static SubStormUltimateAsset GetUnityAsset(this SubStormUltimate data) {
    return data == null ? null : UnityDB.FindAsset<SubStormUltimateAsset>(data);
  }
}
