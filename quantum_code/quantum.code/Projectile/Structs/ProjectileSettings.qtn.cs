using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct ProjectileSettings
    {
        public static ProjectileSettings Lerp(ProjectileSettings a, ProjectileSettings b, FP t)
        {
            return new()
            {
                Prototype = a.Prototype,
                Velocity = FPVector2.Lerp(a.Velocity, b.Velocity, t),
                Offset = FPVector2.Lerp(a.Offset, b.Offset, t)
            };
        }
    }
}
