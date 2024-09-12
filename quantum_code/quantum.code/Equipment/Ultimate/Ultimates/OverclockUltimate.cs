namespace Quantum
{
    [System.Serializable]
    public unsafe partial class OverclockUltimate : Ultimate
    {
        public ApparelStats ApparelStatsMultiplier;
        public WeaponStats WeaponStatsMultiplier;

        public override void OnBegin(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            base.OnBegin(f, ref filter);

            filter.PlayerStats->ApparelStatsMultiplier = ApparelStatsMultiplier;
            filter.PlayerStats->WeaponStatsMultiplier = WeaponStatsMultiplier;
        }

        public override void OnEnd(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.PlayerStats->ApparelStatsMultiplier = ApparelHelper.Default;
            filter.PlayerStats->WeaponStatsMultiplier = WeaponHelper.Default;
        }
    }
}
