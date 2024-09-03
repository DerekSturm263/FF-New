using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    public unsafe partial class MovementSettings
    {
        [Header("General Settings")]
        public FP DeadStickZone;
        public int InputCheckTime;

        public int PlayerCollision;
        public int NoPlayerCollision;

        [Header("Cast Settings")]
        public ShapeCastHelper GroundedCast;
        public ShapeCastHelper WallCastLeft;
        public ShapeCastHelper WallCastRight;
        public ShapeCastHelper CeilingCast;

        [Header("Respawn Settings")]
        public FP RespawnHealRate;
    }
}
