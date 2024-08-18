using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    public unsafe partial class MovementSettings
    {
        [Header("General Settings")]
        public FP DeadStickZone;
        public int InputCheckTime;

        [Header("Movement Settings")]
        public MovementMoveSettings GroundedMoveSettings;
        public MovementMoveSettings AerialMoveSettings;

        [Header("Jump Settings")]
        public MovementCurveSettings ShortHopSettings;
        public MovementCurveSettings FullHopSettings;
        public MovementCurveSettings AerialSettings;

        [Header("Fast Fall Settings")]
        public FP MinimumYVelocity;
        public FP FastFallForce;

        [Header("Block Settings")]
        public FP KnockbackResistance;

        [Header("Sub Settings")]
        public int SubUseTime;

        [Header("Dodge Settings")]
        public MovementCurveSettingsXY SpotDodgeSettings;
        public MovementCurveSettingsXY RollDodgeSettings;
        public MovementCurveSettingsXY AerialDodgeSettings;
        public MovementCurveSettings AerialGravity;

        [Header("Burst Settings")]
        public FP BurstCost;
        public int BurstTime;

        [Header("Interact Settings")]
        public ShapeCastHelper InteractCast;
        public FP InteractCastDistanceMultiplier;
        public int UseTime;

        [Header("Throw Settings")]
        public DirectionalFPVector2 ThrowOffset;
        public FP ThrowForce;
        public FPVector2 ThrowForceOffset;
        public int ThrowTime;

        [Header("Cast Settings")]
        public ShapeCastHelper GroundedCast;
        public ShapeCastHelper WallCastLeft;
        public ShapeCastHelper WallCastRight;
        public ShapeCastHelper CeilingCast;

        [Header("Knockback Settings")]
        public FP DirectionalInfluenceForce;
    }
}
