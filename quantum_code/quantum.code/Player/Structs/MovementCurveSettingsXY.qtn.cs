using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial struct MovementCurveSettingsXY
    {
        public FPAnimationCurve XCurve;
        public FP XForce;

        public FPAnimationCurve YCurve;
        public FP YForce;

        public int Frames;
    }
}
