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

[CreateAssetMenu(menuName = "Quantum/InfoAsset/StagePicker/TurnsStagePicker", order = Quantum.EditorDefines.AssetMenuPriorityStart + 227)]
public partial class TurnsStagePickerAsset : StagePickerAsset {
  public Quantum.TurnsStagePicker Settings_TurnsStagePicker;

  public override string AssetObjectPropertyPath => nameof(Settings_TurnsStagePicker);
  
  public override Quantum.AssetObject AssetObject => Settings_TurnsStagePicker;
  
  public override void Reset() {
    if (Settings_TurnsStagePicker == null) {
      Settings_TurnsStagePicker = new Quantum.TurnsStagePicker();
    }
    base.Reset();
  }
}

public static partial class TurnsStagePickerAssetExts {
  public static TurnsStagePickerAsset GetUnityAsset(this TurnsStagePicker data) {
    return data == null ? null : UnityDB.FindAsset<TurnsStagePickerAsset>(data);
  }
}