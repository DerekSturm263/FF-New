using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ChargingWeaponEnhancer : WeaponEnhancer
    {
        public FP Multiplier;

        public override void OnHit(Frame f, EntityRef user, EntityRef target, HitboxSettings hitbox)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
                StatsSystem.ModifyEnergy(f, user, stats, hitbox.Offensive.Damage / 4 * Multiplier);
        }
    }
}
