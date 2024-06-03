using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    public unsafe partial class MovementSettings
    {
        [Header("Movement Settings")]
        public FP DeadStickZone;
        public MovementMoveSettings GroundedMoveSettings;
        public MovementMoveSettings AerialMoveSettings;
        public FPVector2 FacingLeftDirection;
        public FPVector2 FacingRightDirection;

        [Header("Jump Settings")]
        public int FullHopFrameMin;
        public short MaxJumpCount;
        public MovementCurveSettings ShortHopSettings;
        public MovementCurveSettings FullHopSettings;
        public MovementCurveSettings AerialJumpSettings;
        public MovementCurveSettings WallJumpSettings;

        [Header("Fast Fall Settings")]
        public FP MinimumYVelocity;
        public FP FastFallForce;

        [Header("Block Settings")]
        public FP KnockbackResistance;

        [Header("Dodge Settings")]
        public short MaxDodgeCount;
        public MovementCurveSettings GroundedDodgeSettings;
        public MovementCurveSettings AerialDodgeSettings;

        [Header("Burst Settings")]
        public FP BurstCost;
        public int BurstTime;

        [Header("Throw Settings")]
        public DirectionalFPVector2 ThrowOffset;
        public DirectionalFPVector2 ThrowForce;

        [Header("Knockback Settings")]
        public FP DirectionalInfluenceForce;

        [Header("Input Settings")]
        public int DirectionChangeTime;
        public int SubUseTime;

        [Header("Wall Slide Settings")]
        public FP SlideForce;

        [Header("ShapeCast Settings")]
        public ShapeCastHelper InteractCast;
        public FP InteractCastDistanceMultiplier;
        public ShapeCastHelper GroundedCast;
        public ShapeCastHelper WallCastLeft;
        public ShapeCastHelper WallCastRight;
        public ShapeCastHelper CeilingCast;
    }
}
