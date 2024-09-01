using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial struct MovementCurveSettingsXY
    {
        public FPAnimationCurve XCurve, YCurve;
        public FP XForce, YForce;

        public int Frames;
    }
}
