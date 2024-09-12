using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class LongRangedSubEnhancer : SubEnhancer
    {
        public FP ThrowMultiplier;

        public override void OnApply(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.CharacterController->ThrowMultiplier = ThrowMultiplier;
        }

        public override void OnRemove(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.CharacterController->ThrowMultiplier = 1;
        }
    }
}
