using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class OverheatedStatusEffect : StatusEffect
    {
        public FP Damage;

        public override void OnApply(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.Stats->CurrentStats.Health -= Damage * filter.Stats->StatusEffectMultiplier;
        }

        public override void OnTick(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.Stats->CurrentStats.Health -= Damage * filter.Stats->StatusEffectMultiplier;
        }

        public override void OnRemove(Frame f, ref CharacterControllerSystem.Filter filter)
        {

        }
    }
}
