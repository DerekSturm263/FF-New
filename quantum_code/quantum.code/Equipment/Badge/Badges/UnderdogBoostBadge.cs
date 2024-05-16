using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class UnderdogBoostBadge : Badge
    {
        public override void OnUpdate(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                if (stats->CurrentStocks == 1 || stats->CurrentHealth < 50)
                {
                    stats->ApparelStatsMultiplier = new() { Agility = FP._1_25, Defense = 1, Dodge = 1, Jump = 1, Weight = FP._1_50 };
                    stats->WeaponStatsMultiplier = new() { Damage = FP._1_50, Knockback = FP._1_25, Speed = FP._1_50 };
                }
                else
                {
                    // TODO: FIX BUG WHEN THIS IS USED WITH THE OVERCLOCK ULTIMATE THAT WILL JUST NOT WORK.
                    stats->ApparelStatsMultiplier = ApparelHelper.Default;
                    stats->WeaponStatsMultiplier = WeaponHelper.Default;
                }
            }
        }
    }
}
