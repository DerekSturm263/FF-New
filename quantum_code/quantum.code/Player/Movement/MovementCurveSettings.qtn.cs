using Photon.Deterministic;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quantum
{
    [StructLayout(LayoutKind.Explicit)]
    [System.Serializable]
    public unsafe partial struct MovementCurveSettings
    {
        public const int SIZE = 24;
        public const int ALIGNMENT = 8;

        [FieldOffset(0)]
        public FPAnimationCurve Curve;

        [FieldOffset(8)]
        public FP Force;

        [FieldOffset(16)]
        public int Frames;

        public override bool Equals(object obj)
        {
            return obj is MovementCurveSettings settings &&
                   EqualityComparer<FPAnimationCurve>.Default.Equals(Curve, settings.Curve) &&
                   Force.Equals(settings.Force) &&
                   Frames == settings.Frames;
        }

        public override int GetHashCode()
        {
            int hashCode = -804670169;
            hashCode = hashCode * -1521134295 + Curve.GetHashCode();
            hashCode = hashCode * -1521134295 + Force.GetHashCode();
            hashCode = hashCode * -1521134295 + Frames.GetHashCode();
            return hashCode;
        }
    }
}
