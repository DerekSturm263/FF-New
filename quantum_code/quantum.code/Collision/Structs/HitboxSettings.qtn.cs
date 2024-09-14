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
                HitStun = FPMath.Lerp(a.HitStun, b.HitStun, t).AsInt,
                IFrameTime = FPMath.Lerp(a.IFrameTime, b.IFrameTime, t).AsInt,
                StatusEffect = a.StatusEffect,
                AlignKnockbackToPlayerDirection = a.AlignKnockbackToPlayerDirection
            };
        }

        public static DelaySettings Lerp(DelaySettings a, DelaySettings b, FP t)
        {
            return new()
            {
                FreezeFrames = (uint)FPMath.Lerp(a.FreezeFrames, b.FreezeFrames, t).AsInt,
                ShakeStrength = FPMath.Lerp(a.ShakeStrength, b.ShakeStrength, t)
            };
        }

        public static VisualSettings Lerp(VisualSettings a, VisualSettings b, FP t)
        {
            return new()
            {
                OnlyShakeOnHit = a.OnlyShakeOnHit,
                SpawnHitSparks = a.SpawnHitSparks,
                CameraShake = a.CameraShake,
                TargetShake = a.TargetShake
            };
        }
    }
}
