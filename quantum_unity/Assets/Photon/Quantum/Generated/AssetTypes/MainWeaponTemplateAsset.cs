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

[CreateAssetMenu(menuName = "Quantum/MainWeaponTemplate", order = Quantum.EditorDefines.AssetMenuPriorityStart + 312)]
public partial class MainWeaponTemplateAsset : AssetBase {
  public Quantum.MainWeaponTemplate Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.MainWeaponTemplate();
    }
    base.Reset();
  }
}

public static partial class MainWeaponTemplateAssetExts {
  public static MainWeaponTemplateAsset GetUnityAsset(this MainWeaponTemplate data) {
    return data == null ? null : UnityDB.FindAsset<MainWeaponTemplateAsset>(data);
  }
}
