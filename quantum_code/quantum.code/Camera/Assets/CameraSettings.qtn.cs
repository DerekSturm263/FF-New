using Photon.Deterministic;

namespace Quantum
{
    public partial class CameraSettings
    {
        public int FOV;

        public FP TranslationSpeed;
        public FPVector3 TranslationOffset;
        public FPVector3 TranslationMinClamp;
        public FPVector3 TranslationMaxClamp;

        public FP RotationSpeed;
        public FPVector3 RotationOffset;
        public FPVector3 RotationMinClamp;
        public FPVector3 RotationMaxClamp;

        public FP ZoomOffset;
        public FP ZoomSpeed;
        public FP ZoomScale;

        public FPAnimationCurve ShakeCurve;
        public FP ShakeStrength;
        public FP ShakeFrequency;
    }
}
