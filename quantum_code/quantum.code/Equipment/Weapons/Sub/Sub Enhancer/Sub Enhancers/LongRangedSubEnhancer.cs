using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class LongRangedSubEnhancer : SubEnhancer
    {
        public FP ThrowMultiplier;

        public override void OnApply(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
            {
                characterController->ThrowMultiplier = ThrowMultiplier;
            }
        }

        public override void OnRemove(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
            {
                characterController->ThrowMultiplier = 1;
            }
        }
    }
}
