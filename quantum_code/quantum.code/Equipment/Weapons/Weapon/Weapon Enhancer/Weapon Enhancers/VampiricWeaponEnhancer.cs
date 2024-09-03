
using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class VampiricWeaponEnhancer : WeaponEnhancer
    {
        public FP HealthMultiplier;
        public FP SuccessLevel;

        public override void OnHit(Frame f, EntityRef user, EntityRef target, HitboxSettings hitbox, FP chargeLevel)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats) &&
                f.Unsafe.TryGetPointer(user, out CharacterController* characterController) &&
                chargeLevel >= SuccessLevel)
            {
                stats->CurrentStats.Health += hitbox.Offensive.Damage * HealthMultiplier;
            }
        }
    }
}
