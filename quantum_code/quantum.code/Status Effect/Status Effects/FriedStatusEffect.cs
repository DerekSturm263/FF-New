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
