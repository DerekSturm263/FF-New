using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class UnderdogBoostBadge : Badge
    {
        public FP MinimumStocks;
        public FP MinimumHealth;

        public ApparelStats ApparelStatsMultiplier;
        public WeaponStats WeaponStatsMultiplier;

        public override void OnUpdate(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats) && f.Unsafe.TryGetPointer(user, out PlayerStats* playerStats))
            {
                if (stats->CurrentStats.Stocks <= MinimumStocks || stats->CurrentStats.Health < MinimumHealth)
                {
                    playerStats->ApparelStatsMultiplier = ApparelStatsMultiplier;
                    playerStats->WeaponStatsMultiplier = WeaponStatsMultiplier;
                }
                else
                {
                    // TODO: FIX BUG WHEN THIS IS USED WITH THE OVERCLOCK ULTIMATE THAT WILL JUST NOT WORK.
                    playerStats->ApparelStatsMultiplier = ApparelHelper.Default;
                    playerStats->WeaponStatsMultiplier = WeaponHelper.Default;
                }
            }
        }
    }
}
