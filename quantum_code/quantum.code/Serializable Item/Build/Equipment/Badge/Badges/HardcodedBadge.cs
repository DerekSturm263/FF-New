using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class HardcodedBadge : Badge
    {
        public override void OnApply(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->StatusEffectMultiplier = FP._0_25;
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
