using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ChargingWeaponEnhancer : WeaponEnhancer
    {
        public FP Multiplier;

        public override void OnHit(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef target, HitboxSettings hitbox)
        {
            StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, hitbox.Offensive.Damage / 4 * Multiplier);
        }
    }
}
