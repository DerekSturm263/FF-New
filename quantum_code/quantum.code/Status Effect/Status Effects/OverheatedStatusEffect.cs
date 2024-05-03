using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class OverheatedStatusEffect : StatusEffect
    {
        public FP Damage;

        public override void OnApply(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->CurrentHealth -= Damage * stats->StatusEffectMultiplier;
            }
        }

        public override void OnTick(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->CurrentHealth -= Damage * stats->StatusEffectMultiplier;
            }
        }

        public override void OnRemove(Frame f, EntityRef user)
        {

        }
    }
}
