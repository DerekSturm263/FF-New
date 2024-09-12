using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class VampiricWeaponEnhancer : WeaponEnhancer
    {
        public FP HealthMultiplier;
        public FP SuccessLevel;

        public override void OnHit(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef target, HitboxSettings hitbox)
        {
            if (filter.CharacterController->HoldLevel >= SuccessLevel)
            {
                StatsSystem.ModifyHealth(f, filter.Entity, filter.Stats, hitbox.Offensive.Damage * HealthMultiplier, false);
            }
        }
    }
}
