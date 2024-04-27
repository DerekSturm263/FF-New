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

[CreateAssetMenu(menuName = "Quantum/Item", order = Quantum.EditorDefines.AssetMenuPriorityStart + 208)]
public partial class ItemAsset : AssetBase {
  public Quantum.Item Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.Item();
    }
    base.Reset();
  }
}

public static partial class ItemAssetExts {
  public static ItemAsset GetUnityAsset(this Item data) {
    return data == null ? null : UnityDB.FindAsset<ItemAsset>(data);
  }
}
