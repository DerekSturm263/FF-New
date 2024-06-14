// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial  
// declarations in another file.
// </auto-generated>

#if UNITY_EDITOR
namespace Quantum.Editor {
  using Quantum.Prototypes;
  using UnityEditor;
  using UnityEngine;

  internal sealed class QuantumEditorUtilityContainer : ScriptableSingleton<QuantumEditorUtilityContainer> {

    public static new QuantumEditorUtilityContainer instance {
      get {
        var result = ScriptableSingleton<QuantumEditorUtilityContainer>.instance;
        result.hideFlags = HideFlags.None;
        return result;
      }
    }

    public KnownObjectsContainer ObjectsContainer = new KnownObjectsContainer();
    public FlatEntityPrototypeContainer PendingPrototype = new FlatEntityPrototypeContainer();

    [System.Serializable]
    public partial class KnownObjectsContainer : QuantumEditorUtility.SerializableObjectsContainerBase {
      public Quantum.Voice[] Voice = {};
      public Quantum.NoneBadge[] NoneBadge = {};
      public Quantum.DigitalDoubleUltimate[] DigitalDoubleUltimate = {};
      public Quantum.NoneUltimate[] NoneUltimate = {};
      public Quantum.UpgradeUltimate[] UpgradeUltimate = {};
      public Quantum.LongRangedSubEnhancer[] LongRangedSubEnhancer = {};
      public Quantum.BouncySubEnhancer[] BouncySubEnhancer = {};
      public Quantum.NoneSubEnhancer[] NoneSubEnhancer = {};
      public Quantum.NoneWeaponEnhancer[] NoneWeaponEnhancer = {};
      public Quantum.EnergyBeamWeaponEnhancer[] EnergyBeamWeaponEnhancer = {};
      public Quantum.ShockwaveWeaponEnhancer[] ShockwaveWeaponEnhancer = {};
      public Quantum.Behavior[] Behavior = {};
      public Quantum.TrackGraph[] TrackGraph = {};
      public Quantum.EnergyConversionBadge[] EnergyConversionBadge = {};
      public Quantum.InfoAsset[] InfoAsset = {};
      public Quantum.SeekingExplodingItem[] SeekingExplodingItem = {};
      public Quantum.SeekingItem[] SeekingItem = {};
      public Quantum.ExplodingItem[] ExplodingItem = {};
      public Quantum.HealingItem[] HealingItem = {};
      public Quantum.MatchAsset[] MatchAsset = {};
      public Quantum.QuantumAnimationEvent[] QuantumAnimationEvent = {};
      public Quantum.MovementSettings[] MovementSettings = {};
      public Quantum.BuildAsset[] BuildAsset = {};
      public Quantum.FFAvatar[] FFAvatar = {};
      public Quantum.Emote[] Emote = {};
      public Quantum.Eyes[] Eyes = {};
      public Quantum.Hair[] Hair = {};
      public Quantum.ApparelModifier[] ApparelModifier = {};
      public Quantum.ApparelPattern[] ApparelPattern = {};
      public Quantum.ApparelTemplate[] ApparelTemplate = {};
      public Quantum.HardcodedBadge[] HardcodedBadge = {};
      public Quantum.MagneticBadge[] MagneticBadge = {};
      public Quantum.UnderdogBoostBadge[] UnderdogBoostBadge = {};
      public Quantum.OverclockUltimate[] OverclockUltimate = {};
      public Quantum.SubStormUltimate[] SubStormUltimate = {};
      public Quantum.ChargingWeaponEnhancer[] ChargingWeaponEnhancer = {};
      public Quantum.VampiricWeaponEnhancer[] VampiricWeaponEnhancer = {};
      public Quantum.WeaponMaterial[] WeaponMaterial = {};
      public Quantum.WeaponTemplate[] WeaponTemplate = {};
      public Quantum.ElectrifiedSubEnhancer[] ElectrifiedSubEnhancer = {};
      public Quantum.PiercingSubEnhancer[] PiercingSubEnhancer = {};
      public Quantum.SubTemplate[] SubTemplate = {};
      public Quantum.RulesetAsset[] RulesetAsset = {};
      public Quantum.BestKillToDeathRatioWinCondition[] BestKillToDeathRatioWinCondition = {};
      public Quantum.LeastDeathsWinCondition[] LeastDeathsWinCondition = {};
      public Quantum.MostKillsWinCondition[] MostKillsWinCondition = {};
      public Quantum.Gizmo[] Gizmo = {};
      public Quantum.StageAsset[] StageAsset = {};
      public Quantum.FriedStatusEffect[] FriedStatusEffect = {};
      public Quantum.OverheatedStatusEffect[] OverheatedStatusEffect = {};
      public Quantum.ApplyPhysicsEvent[] ApplyPhysicsEvent = {};
      public Quantum.ModifyHurtboxesEvent[] ModifyHurtboxesEvent = {};
      public Quantum.SpawnProjectileEvent[] SpawnProjectileEvent = {};
      public Quantum.SpawnHitboxEvent[] SpawnHitboxEvent = {};
      public Quantum.CameraSettings[] CameraSettings = {};
      public Quantum.CustomAnimatorGraph[] CustomAnimatorGraph = {};
      public Quantum.SimulationConfig[] SimulationConfig = {};
      public Quantum.BinaryData[] BinaryData = {};
      public Quantum.CharacterController2DConfig[] CharacterController2DConfig = {};
      public Quantum.CharacterController3DConfig[] CharacterController3DConfig = {};
      public Quantum.EntityPrototype[] EntityPrototype = {};
      public Quantum.EntityView[] EntityView = {};
      public Quantum.Map[] Map = {};
      public Quantum.NavMesh[] NavMesh = {};
      public Quantum.NavMeshAgentConfig[] NavMeshAgentConfig = {};
      public Quantum.PhysicsMaterial[] PhysicsMaterial = {};
      public Quantum.PolygonCollider[] PolygonCollider = {};
      public Quantum.TerrainCollider[] TerrainCollider = {};

    }
  }
}
#endif
