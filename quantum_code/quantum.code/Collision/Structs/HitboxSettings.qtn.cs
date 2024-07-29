using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct HitboxSettings
    {
        public static HitboxSettings Lerp(HitboxSettings a, HitboxSettings b, FP t)
        {
            return new()
            {
                Parent = a.Parent,
                Damage = FPMath.Lerp(a.Damage, b.Damage, t),
                Knockback = FPVector2.Lerp(a.Knockback, b.Knockback, t),
                UserFreezeDuration = (uint)FPMath.Lerp(a.UserFreezeDuration, b.UserFreezeDuration, t).AsInt,
                TargetFreezeDuration = (uint)FPMath.Lerp(a.TargetFreezeDuration, b.TargetFreezeDuration, t).AsInt,
                TargetShakeIntensity = FPMath.Lerp(a.TargetShakeIntensity, b.TargetShakeIntensity, t),
                StatusEffect = a.StatusEffect,
                SpawnShake = a.SpawnShake,
                HitShake = a.HitShake,
            };
        }
    }
}
