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

[CreateAssetMenu(menuName = "Quantum/InfoAsset/WeaponEnhancer/ShockwaveWeaponEnhancer", order = Quantum.EditorDefines.AssetMenuPriorityStart + 226)]
public partial class ShockwaveWeaponEnhancerAsset : WeaponEnhancerAsset {
  public Quantum.ShockwaveWeaponEnhancer Settings_ShockwaveWeaponEnhancer;

  public override string AssetObjectPropertyPath => nameof(Settings_ShockwaveWeaponEnhancer);
  
  public override Quantum.AssetObject AssetObject => Settings_ShockwaveWeaponEnhancer;
  
  public override void Reset() {
    if (Settings_ShockwaveWeaponEnhancer == null) {
      Settings_ShockwaveWeaponEnhancer = new Quantum.ShockwaveWeaponEnhancer();
    }
    base.Reset();
  }
}

public static partial class ShockwaveWeaponEnhancerAssetExts {
  public static ShockwaveWeaponEnhancerAsset GetUnityAsset(this ShockwaveWeaponEnhancer data) {
    return data == null ? null : UnityDB.FindAsset<ShockwaveWeaponEnhancerAsset>(data);
  }
}