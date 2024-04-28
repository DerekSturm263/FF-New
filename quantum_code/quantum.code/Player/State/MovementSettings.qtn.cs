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
        public MovementCurveSettings GroundedDodgeSettings;
        public MovementCurveSettings AerialDodgeSettings;

        [Header("Burst Settings")]
        public FP BurstCost;

        [Header("Interact Settings")]
        public ShapeCastHelper InteractCast;

        [Header("Throw Settings")]
        public FPVector2 ThrowAmount;

        [Header("Knockback Settings")]
        public FP DirectionalInfluenceForce;

        [Header("Direction Settings")]
        public int DirectionChangeTime;

        [Header("Wall Slide Settings")]
        public FP SlideForce;

        [Header("ShapeCast Settings")]
        public ShapeCastHelper GroundedCast;
        public ShapeCastHelper WallCastLeft;
        public ShapeCastHelper WallCastRight;
        public ShapeCastHelper CeilingCast;
    }
}
