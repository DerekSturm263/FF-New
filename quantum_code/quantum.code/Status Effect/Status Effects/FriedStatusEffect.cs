using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class FriedStatusEffect : StatusEffect
    {
        public override void OnApply(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
            {
                characterController->CanInput = false;
            }
            
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->StatusEffectTime *= stats->StatusEffectMultiplier;
            }
        }

        public override void OnRemove(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
            {
                characterController->CanInput = true;
            }
        }
    }
}
