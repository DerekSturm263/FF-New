using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct HitboxSettings
    {
        public static HitboxSettings Lerp(HitboxSettings a, HitboxSettings b, FP t)
        {
            return new()
            {
                Offensive = Lerp(a.Offensive, b.Offensive, t),
                Delay = Lerp(a.Delay, b.Delay, t),
                Visual = Lerp(a.Visual, b.Visual, t)
            };
        }

        public static OffensiveSettings Lerp(OffensiveSettings a, OffensiveSettings b, FP t)
        {
            return new()
            {
                Damage = FPMath.Lerp(a.Damage, b.Damage, t),
                Knockback = FPVector2.Lerp(a.Knockback, b.Knockback, t),
                StatusEffect = a.StatusEffect
            };
        }

        public static DelaySettings Lerp(DelaySettings a, DelaySettings b, FP t)
        {
            return new()
            {
                UserFreezeFrames = (uint)FPMath.Lerp(a.UserFreezeFrames, b.UserFreezeFrames, t).AsInt,
                TargetFreezeFrames = (uint)FPMath.Lerp(a.TargetFreezeFrames, b.TargetFreezeFrames, t).AsInt
            };
        }

        public static VisualSettings Lerp(VisualSettings a, VisualSettings b, FP t)
        {
            return new()
            {
                CameraShake = a.CameraShake,
                TargetShake = a.TargetShake
            };
        }
    }
}
