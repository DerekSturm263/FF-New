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

[CreateAssetMenu(menuName = "Quantum/SubWeaponEnhancer/ElectrifiedSubWeaponEnhancer", order = Quantum.EditorDefines.AssetMenuPriorityStart + 472)]
public partial class ElectrifiedSubWeaponEnhancerAsset : SubWeaponEnhancerAsset {
  public Quantum.ElectrifiedSubWeaponEnhancer Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.ElectrifiedSubWeaponEnhancer();
    }
    base.Reset();
  }
}

public static partial class ElectrifiedSubWeaponEnhancerAssetExts {
  public static ElectrifiedSubWeaponEnhancerAsset GetUnityAsset(this ElectrifiedSubWeaponEnhancer data) {
    return data == null ? null : UnityDB.FindAsset<ElectrifiedSubWeaponEnhancerAsset>(data);
  }
}