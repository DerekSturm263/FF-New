using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class HardcodedBadge : Badge
    {
        public FP StatusEffectMultiplier;

        public override void OnApply(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->StatusEffectMultiplier = StatusEffectMultiplier;
            }
        }

        public override void OnRemove(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->StatusEffectMultiplier = 1;
            }
        }
    }
}
