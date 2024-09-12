using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class HardcodedBadge : Badge
    {
        public FP StatusEffectMultiplier;

        public override void OnApply(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.Stats->StatusEffectMultiplier = StatusEffectMultiplier;
        }

        public override void OnRemove(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.Stats->StatusEffectMultiplier = 1;
        }
    }
}
