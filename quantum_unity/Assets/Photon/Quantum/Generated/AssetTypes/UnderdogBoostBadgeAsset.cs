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

[CreateAssetMenu(menuName = "Quantum/InfoAsset/Badge/UnderdogBoostBadge", order = Quantum.EditorDefines.AssetMenuPriorityStart + 228)]
public partial class UnderdogBoostBadgeAsset : BadgeAsset {
  public Quantum.UnderdogBoostBadge Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.UnderdogBoostBadge();
    }
    base.Reset();
  }
}

public static partial class UnderdogBoostBadgeAssetExts {
  public static UnderdogBoostBadgeAsset GetUnityAsset(this UnderdogBoostBadge data) {
    return data == null ? null : UnityDB.FindAsset<UnderdogBoostBadgeAsset>(data);
  }
}
