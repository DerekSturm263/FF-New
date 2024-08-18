using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial struct MovementCurveSettings
    {
        public FPAnimationCurve Curve;
        public FP Force;

        public int Frames;
    }
}
