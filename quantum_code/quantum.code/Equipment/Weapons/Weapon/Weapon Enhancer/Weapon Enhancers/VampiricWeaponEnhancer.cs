using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class VampiricWeaponEnhancer : WeaponEnhancer
    {
        public FP HealthMultiplier;
        public FP SuccessLevel;

        public override void OnHit(Frame f, EntityRef user, EntityRef target, HitboxSettings hitbox)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats) &&
                f.Unsafe.TryGetPointer(user, out CharacterController* characterController) &&
                characterController->HoldLevel >= SuccessLevel)
            {
                StatsSystem.ModifyHealth(f, user, stats, hitbox.Offensive.Damage * HealthMultiplier, false);
            }
        }
    }
}
