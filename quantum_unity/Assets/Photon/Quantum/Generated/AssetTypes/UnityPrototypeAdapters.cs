// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
// </auto-generated>

using System;
namespace Quantum.Prototypes.Unity {
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.AIData))]
  public class AIData_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.AIData_Prototype> {
    public Quantum.AssetRefBehavior Behavior;
    [Quantum.LocalReference]
    public global::EntityPrototype Target;
    public Quantum.Prototypes.Goal_Prototype CurrentGoal;
    public Photon.Deterministic.FP TimeSinceAction;

    public sealed override Quantum.Prototypes.AIData_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.AIData_Prototype();
      result.Behavior = this.Behavior;
      converter.Convert(this.Target, out result.Target);
      result.CurrentGoal = this.CurrentGoal;
      result.TimeSinceAction = this.TimeSinceAction;
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.HitboxInstance))]
  public class HitboxInstance_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.HitboxInstance_Prototype> {
    public System.Int32 PathQueryIndex;
    public System.Int32 Lifetime;
    public Quantum.Prototypes.HitboxSettings_Prototype Settings;
    [Quantum.LocalReference]
    public global::EntityPrototype Owner;
    [Quantum.LocalReference]
    public global::EntityPrototype Parent;

    public sealed override Quantum.Prototypes.HitboxInstance_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.HitboxInstance_Prototype();
      result.PathQueryIndex = this.PathQueryIndex;
      result.Lifetime = this.Lifetime;
      result.Settings = this.Settings;
      converter.Convert(this.Owner, out result.Owner);
      converter.Convert(this.Parent, out result.Parent);
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.HurtboxInstance))]
  public class HurtboxInstance_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.HurtboxInstance_Prototype> {
    public Quantum.Prototypes.HurtboxSettings_Prototype Settings;
    public System.Int32 Index;
    [Quantum.LocalReference]
    public global::EntityPrototype Owner;

    public sealed override Quantum.Prototypes.HurtboxInstance_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.HurtboxInstance_Prototype();
      result.Settings = this.Settings;
      result.Index = this.Index;
      converter.Convert(this.Owner, out result.Owner);
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.ItemInstance))]
  public class ItemInstance_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.ItemInstance_Prototype> {
    public Quantum.AssetRefItem Item;
    [Quantum.LocalReference]
    public global::EntityPrototype Holder;
    [Quantum.LocalReference]
    public global::EntityPrototype Owner;
    [Quantum.LocalReference]
    public global::EntityPrototype Target;
    public Quantum.QBoolean FallState;
    public Photon.Deterministic.FP FallSpeed;
    public Photon.Deterministic.FP FallY;
    public System.Int32 Uses;

    public sealed override Quantum.Prototypes.ItemInstance_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.ItemInstance_Prototype();
      result.Item = this.Item;
      converter.Convert(this.Holder, out result.Holder);
      converter.Convert(this.Owner, out result.Owner);
      converter.Convert(this.Target, out result.Target);
      result.FallState = this.FallState;
      result.FallSpeed = this.FallSpeed;
      result.FallY = this.FallY;
      result.Uses = this.Uses;
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.MatchInstance))]
  public class MatchInstance_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.MatchInstance_Prototype> {
    public Match_Prototype Match;
    public Quantum.QBoolean IsTimerOver;
    public Quantum.QBoolean IsMatchRunning;
    [Quantum.LocalReference]
    public global::EntityPrototype CurrentStage;

    public sealed override Quantum.Prototypes.MatchInstance_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.MatchInstance_Prototype();
      result.Match = this.Match.Convert(converter);
      result.IsTimerOver = this.IsTimerOver;
      result.IsMatchRunning = this.IsMatchRunning;
      converter.Convert(this.CurrentStage, out result.CurrentStage);
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.Match))]
  public class Match_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.Match_Prototype> {
    public Quantum.Prototypes.Ruleset_Prototype Ruleset;
    public Quantum.Prototypes.Stage_Prototype Stage;
    [Quantum.Inspector.DynamicCollectionAttribute()]
    public Team_Prototype[] Teams = System.Array.Empty<Team_Prototype>();

    public sealed override Quantum.Prototypes.Match_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.Match_Prototype();
      result.Ruleset = this.Ruleset;
      result.Stage = this.Stage;
      result.Teams = System.Array.ConvertAll(this.Teams, x => x.Convert(converter));
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.Team))]
  public class Team_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.Team_Prototype> {
    [Quantum.Inspector.DynamicCollectionAttribute()]
    public PlayerLink_Prototype[] Players = System.Array.Empty<PlayerLink_Prototype>();

    public sealed override Quantum.Prototypes.Team_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.Team_Prototype();
      result.Players = System.Array.ConvertAll(this.Players, x => x.Convert(converter));
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.PlayerLink))]
  public class PlayerLink_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.PlayerLink_Prototype> {
    public Quantum.PlayerRef Player;
    [Quantum.LocalReference]
    public global::EntityPrototype Entity;

    public sealed override Quantum.Prototypes.PlayerLink_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.PlayerLink_Prototype();
      result.Player = this.Player;
      converter.Convert(this.Entity, out result.Entity);
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.Stats))]
  public class Stats_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.Stats_Prototype> {
    public Quantum.Prototypes.Build_Prototype Build;
    [Quantum.Inspector.DynamicCollectionAttribute()]
    [Quantum.LocalReference]
    public global::EntityPrototype[] Hitboxes = System.Array.Empty<global::EntityPrototype>();
    [Quantum.Inspector.DictionaryAttribute()]
    [Quantum.Inspector.DynamicCollectionAttribute()]
    public DictionaryEntry_HurtboxType_EntityRef_Prototype[] Hurtboxes = System.Array.Empty<DictionaryEntry_HurtboxType_EntityRef_Prototype>();
    public Quantum.AssetRefEntityPrototype Hurtbox;
    public System.Int32 CurrentStocks;
    public Photon.Deterministic.FP CurrentHealth;
    public Photon.Deterministic.FP HealthModifyMultiplier;
    public Photon.Deterministic.FP CurrentEnergy;
    public Photon.Deterministic.FP EnergyModifyMultiplier;
    public System.Int32 Kills;
    public System.Int32 Deaths;
    public Quantum.Prototypes.ApparelStats_Prototype ApparelStatsMultiplier;
    public Quantum.Prototypes.WeaponStats_Prototype WeaponStatsMultiplier;
    [Quantum.LocalReference]
    public global::EntityPrototype HeldItem;
    public Quantum.AssetRefStatusEffect StatusEffect;
    public System.Int32 StatusEffectTimeLeft;
    public Photon.Deterministic.FP StatusEffectMultiplier;

    public sealed override Quantum.Prototypes.Stats_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.Stats_Prototype();
      result.Build = this.Build;
      result.Hitboxes = System.Array.ConvertAll(this.Hitboxes, x => { converter.Convert(x, out Quantum.MapEntityId tmp); return tmp; });
      result.Hurtboxes = System.Array.ConvertAll(this.Hurtboxes, x => x.Convert(converter));
      result.Hurtbox = this.Hurtbox;
      result.CurrentStocks = this.CurrentStocks;
      result.CurrentHealth = this.CurrentHealth;
      result.HealthModifyMultiplier = this.HealthModifyMultiplier;
      result.CurrentEnergy = this.CurrentEnergy;
      result.EnergyModifyMultiplier = this.EnergyModifyMultiplier;
      result.Kills = this.Kills;
      result.Deaths = this.Deaths;
      result.ApparelStatsMultiplier = this.ApparelStatsMultiplier;
      result.WeaponStatsMultiplier = this.WeaponStatsMultiplier;
      converter.Convert(this.HeldItem, out result.HeldItem);
      result.StatusEffect = this.StatusEffect;
      result.StatusEffectTimeLeft = this.StatusEffectTimeLeft;
      result.StatusEffectMultiplier = this.StatusEffectMultiplier;
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(System.Collections.Generic.KeyValuePair<Quantum.HurtboxType, Quantum.EntityRef>))]
  public class DictionaryEntry_HurtboxType_EntityRef_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.DictionaryEntry_HurtboxType_EntityRef_Prototype> {
    public Quantum.Prototypes.HurtboxType_Prototype Key;
    [Quantum.LocalReference]
    public global::EntityPrototype Value;

    public sealed override Quantum.Prototypes.DictionaryEntry_HurtboxType_EntityRef_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.DictionaryEntry_HurtboxType_EntityRef_Prototype();
      result.Key = this.Key;
      converter.Convert(this.Value, out result.Value);
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.PhysicsJoints3D))]
  public class PhysicsJoints3D_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.PhysicsJoints3D_Prototype> {
    [Quantum.Inspector.DynamicCollectionAttribute()]
    public Joint3D_Prototype[] JointConfigs = System.Array.Empty<Joint3D_Prototype>();

    public sealed override Quantum.Prototypes.PhysicsJoints3D_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.PhysicsJoints3D_Prototype();
      result.JointConfigs = System.Array.ConvertAll(this.JointConfigs, x => x.Convert(converter));
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.Physics3D.Joint3D))]
  public class Joint3D_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.Joint3D_Prototype> {
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("If the joint should be materialized with Enabled set to false, not being considered by the Physics Engine.")]
    public System.Boolean StartDisabled;
    [Quantum.Inspector.DisplayNameAttribute("Type")]
    [Quantum.Inspector.TooltipAttribute("The type of the joint, implying which constraints are applied.")]
    public Quantum.Physics3D.JointType3D JointType;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("A numerical tag that can be used to identify a joint or a group of joints.")]
    public System.Int32 UserTag;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("A Map Entity that the joint might be connected to.\nThe entity must have at least a transform component.")]
    [Quantum.LocalReference]
    public global::EntityPrototype ConnectedEntity;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The anchor point to which the joint connects to.\nIf a Connected Entity is provided, this represents an offset in its local space. Otherwise, the connected anchor is a position in world space.")]
    public Photon.Deterministic.FPVector3 ConnectedAnchor;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The anchor offset, in the local space of this joint entity's transform.\nThis is the point considered for the joint constraints and where the forces will be applied in the joint entity's body.")]
    public Photon.Deterministic.FPVector3 Anchor;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("Axis around which the joint rotates, defined in the local space of the entity.\nThe vector is normalized before set. If zeroed, FPVector3.Right is used instead.")]
    public Photon.Deterministic.FPVector3 Axis;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)2, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The frequency in Hertz (Hz) at which the spring joint will attempt to oscillate.\nTypical values are below half the frequency of the simulation.")]
    public Photon.Deterministic.FP Frequency;
    [Quantum.Inspector.RangeAttribute((Single)0, (Single)2)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)2, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("A dimensionless value representing the damper capacity of suppressing the spring oscillation, typically between 0 and 1.")]
    public Photon.Deterministic.FP DampingRatio;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("Automatically configure the target Distance to be the current distance between the anchor points in the scene.")]
    public System.Boolean AutoConfigureDistance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)2, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("AutoConfigureDistance", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)2)]
    [Quantum.Inspector.TooltipAttribute("The distance between the anchor points that the joint will attempt to maintain.")]
    public Photon.Deterministic.FP Distance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("AutoConfigureDistance", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)2)]
    [Quantum.Inspector.TooltipAttribute("The minimum distance between the anchor points that the joint will attempt to maintain.")]
    public Photon.Deterministic.FP MinDistance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("AutoConfigureDistance", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)2)]
    [Quantum.Inspector.TooltipAttribute("The maximum distance between the anchor points that the joint will attempt to maintain.")]
    public Photon.Deterministic.FP MaxDistance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("If the relative angle between the joint transform and its connected anchor should be limited by the hinge joint.\nSet this checkbox to configure the lower and upper limiting angles.")]
    public System.Boolean UseAngleLimits;
    [Quantum.Inspector.DrawIfAttribute("UseAngleLimits", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The lower limiting angle of the allowed arc of rotation around the connected anchor, in degrees.")]
    public Photon.Deterministic.FP LowerAngle;
    [Quantum.Inspector.DrawIfAttribute("UseAngleLimits", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The upper limiting  angle of the allowed arc of rotation around the connected anchor, in degrees.")]
    public Photon.Deterministic.FP UpperAngle;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("If the hinge joint uses a motor.\nSet this checkbox to configure the motor speed and max torque.")]
    public System.Boolean UseMotor;
    [Quantum.Inspector.DrawIfAttribute("UseMotor", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The speed at which the hinge motor will attempt to rotate, in angles per second.")]
    public Photon.Deterministic.FP MotorSpeed;
    [Quantum.Inspector.DrawIfAttribute("UseMotor", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The maximum torque produced by the hinge motor in order to achieve the target motor speed.\nLeave this checkbox unchecked and the motor toque should not be limited.")]
    public Photon.Deterministic.NullableFP MaxMotorTorque;

    public sealed override Quantum.Prototypes.Joint3D_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.Joint3D_Prototype();
      result.StartDisabled = this.StartDisabled;
      result.JointType = this.JointType;
      result.UserTag = this.UserTag;
      converter.Convert(this.ConnectedEntity, out result.ConnectedEntity);
      result.ConnectedAnchor = this.ConnectedAnchor;
      result.Anchor = this.Anchor;
      result.Axis = this.Axis;
      result.Frequency = this.Frequency;
      result.DampingRatio = this.DampingRatio;
      result.AutoConfigureDistance = this.AutoConfigureDistance;
      result.Distance = this.Distance;
      result.MinDistance = this.MinDistance;
      result.MaxDistance = this.MaxDistance;
      result.UseAngleLimits = this.UseAngleLimits;
      result.LowerAngle = this.LowerAngle;
      result.UpperAngle = this.UpperAngle;
      result.UseMotor = this.UseMotor;
      result.MotorSpeed = this.MotorSpeed;
      result.MaxMotorTorque = this.MaxMotorTorque;
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.PhysicsJoints2D))]
  public class PhysicsJoints2D_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.PhysicsJoints2D_Prototype> {
    [Quantum.Inspector.DynamicCollectionAttribute()]
    public Joint2D_Prototype[] JointConfigs = System.Array.Empty<Joint2D_Prototype>();

    public sealed override Quantum.Prototypes.PhysicsJoints2D_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.PhysicsJoints2D_Prototype();
      result.JointConfigs = System.Array.ConvertAll(this.JointConfigs, x => x.Convert(converter));
      return result;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.PrototypeAttribute(typeof(Quantum.Physics2D.Joint))]
  public class Joint2D_Prototype : Quantum.PrototypeAdapter<Quantum.Prototypes.Joint2D_Prototype> {
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("If the joint should be materialized with Enabled set to false, not being considered by the Physics Engine.")]
    public System.Boolean StartDisabled;
    [Quantum.Inspector.DisplayNameAttribute("Type")]
    [Quantum.Inspector.TooltipAttribute("The type of the joint, implying which constraints are applied.")]
    public Quantum.Physics2D.JointType JointType;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("A numerical tag that can be used to identify a joint or a group of joints.")]
    public System.Int32 UserTag;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("A Map Entity that the joint might be connected to.\nThe entity must have at least a Transform2D component.")]
    [Quantum.LocalReference]
    public global::EntityPrototype ConnectedEntity;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The anchor point to which the joint connects to.\nIf a Connected Entity is provided, this represents an offset in its local space. Otherwise, the connected anchor is a position in world space.")]
    public Photon.Deterministic.FPVector2 ConnectedAnchor;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The anchor offset, in the local space of this joint entity's transform.\nThis is the point considered for the joint constraints and where the forces will be applied in the joint entity's body.")]
    public Photon.Deterministic.FPVector2 Anchor;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)2, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The frequency in Hertz (Hz) at which the spring joint will attempt to oscillate.\nTypical values are below half the frequency of the simulation.")]
    public Photon.Deterministic.FP Frequency;
    [Quantum.Inspector.RangeAttribute((Single)0, (Single)2)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)2, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("A dimensionless value representing the damper capacity of suppressing the spring oscillation, typically between 0 and 1.")]
    public Photon.Deterministic.FP DampingRatio;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)1, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("Automatically configure the target Distance to be the current distance between the anchor points in the scene.")]
    public System.Boolean AutoConfigureDistance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)2, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("AutoConfigureDistance", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)2)]
    [Quantum.Inspector.TooltipAttribute("The distance between the anchor points that the joint will attempt to maintain.")]
    public Photon.Deterministic.FP Distance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("AutoConfigureDistance", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)2)]
    [Quantum.Inspector.TooltipAttribute("The minimum distance between the anchor points that the joint will attempt to maintain.")]
    public Photon.Deterministic.FP MinDistance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("AutoConfigureDistance", (Int64)0, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)2)]
    [Quantum.Inspector.TooltipAttribute("The maximum distance between the anchor points that the joint will attempt to maintain.")]
    public Photon.Deterministic.FP MaxDistance;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("If the relative angle between the joint transform and its connected anchor should be limited by the hinge joint.\nSet this checkbox to configure the lower and upper limiting angles.")]
    public System.Boolean UseAngleLimits;
    [Quantum.Inspector.DegreesAttribute()]
    [Quantum.Inspector.DrawIfAttribute("UseAngleLimits", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The lower limiting angle of the allowed arc of rotation around the connected anchor, in degrees.")]
    public Photon.Deterministic.FP LowerAngle;
    [Quantum.Inspector.DegreesAttribute()]
    [Quantum.Inspector.DrawIfAttribute("UseAngleLimits", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The upper limiting  angle of the allowed arc of rotation around the connected anchor, in degrees.")]
    public Photon.Deterministic.FP UpperAngle;
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("If the hinge joint uses a motor.\nSet this checkbox to configure the motor speed and max torque.")]
    public System.Boolean UseMotor;
    [Quantum.Inspector.DegreesAttribute()]
    [Quantum.Inspector.DrawIfAttribute("UseMotor", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The speed at which the hinge motor will attempt to rotate, in angles per second.")]
    public Photon.Deterministic.FP MotorSpeed;
    [Quantum.Inspector.DrawIfAttribute("UseMotor", (Int64)1, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.DrawIfAttribute("JointType", (Int64)3, (Quantum.Inspector.DrawIfCompareOperator)0, (Quantum.Inspector.DrawIfHideType)3)]
    [Quantum.Inspector.TooltipAttribute("The maximum torque produced by the hinge motor in order to achieve the target motor speed.\nLeave this checkbox unchecked and the motor toque should not be limited.")]
    public Photon.Deterministic.NullableFP MaxMotorTorque;

    public sealed override Quantum.Prototypes.Joint2D_Prototype Convert(EntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.Joint2D_Prototype();
      result.StartDisabled = this.StartDisabled;
      result.JointType = this.JointType;
      result.UserTag = this.UserTag;
      converter.Convert(this.ConnectedEntity, out result.ConnectedEntity);
      result.ConnectedAnchor = this.ConnectedAnchor;
      result.Anchor = this.Anchor;
      result.Frequency = this.Frequency;
      result.DampingRatio = this.DampingRatio;
      result.AutoConfigureDistance = this.AutoConfigureDistance;
      result.Distance = this.Distance;
      result.MinDistance = this.MinDistance;
      result.MaxDistance = this.MaxDistance;
      result.UseAngleLimits = this.UseAngleLimits;
      result.LowerAngle = this.LowerAngle;
      result.UpperAngle = this.UpperAngle;
      result.UseMotor = this.UseMotor;
      result.MotorSpeed = this.MotorSpeed;
      result.MaxMotorTorque = this.MaxMotorTorque;
      return result;
    }
  }

}
