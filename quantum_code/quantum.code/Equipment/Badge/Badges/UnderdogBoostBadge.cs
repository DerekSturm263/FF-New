namespace Quantum
{
    [System.Serializable]
    public unsafe partial class UnderdogBoostBadge : Badge
    {
        public ApparelStats ApparelStatsMultiplier;
        public WeaponStats WeaponStatsMultiplier;

        public override void OnUpdate(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            if (filter.Stats->CurrentStats.Stocks <= f.Global->CurrentMatch.Ruleset.Players.StockCount / 2)
            {
                filter.PlayerStats->ApparelStatsMultiplier = ApparelStatsMultiplier;
                filter.PlayerStats->WeaponStatsMultiplier = WeaponStatsMultiplier;
            }
            else
            {
                // TODO: FIX BUG WHEN THIS IS USED WITH THE OVERCLOCK ULTIMATE THAT WILL JUST NOT WORK.
                filter.PlayerStats->ApparelStatsMultiplier = ApparelHelper.Default;
                filter.PlayerStats->WeaponStatsMultiplier = WeaponHelper.Default;
            }
        }
    }
}
