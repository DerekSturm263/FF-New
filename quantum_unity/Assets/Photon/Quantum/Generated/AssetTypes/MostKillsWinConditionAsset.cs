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

[CreateAssetMenu(menuName = "Quantum/WinCondition/MostKillsWinCondition", order = Quantum.EditorDefines.AssetMenuPriorityStart + 584)]
public partial class MostKillsWinConditionAsset : WinConditionAsset {
  public Quantum.MostKillsWinCondition Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.MostKillsWinCondition();
    }
    base.Reset();
  }
}

public static partial class MostKillsWinConditionAssetExts {
  public static MostKillsWinConditionAsset GetUnityAsset(this MostKillsWinCondition data) {
    return data == null ? null : UnityDB.FindAsset<MostKillsWinConditionAsset>(data);
  }
}