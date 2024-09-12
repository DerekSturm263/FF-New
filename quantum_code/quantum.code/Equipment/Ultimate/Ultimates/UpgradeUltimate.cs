namespace Quantum
{
    [System.Serializable]
    public unsafe partial class UpgradeUltimate : Ultimate
    {
        public ApparelStats ApparelStatsMultiplier;
        public WeaponStats WeaponStatsMultiplier;

        public override void OnBegin(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            base.OnBegin(f, ref filter);

            filter.PlayerStats->ApparelStatsMultiplier = ApparelHelper.Multiply(filter.PlayerStats->ApparelStatsMultiplier, ApparelStatsMultiplier);
            filter.PlayerStats->WeaponStatsMultiplier = WeaponHelper.Multiply(filter.PlayerStats->WeaponStatsMultiplier, WeaponStatsMultiplier);
        }

        public override void OnEnd(Frame f, ref CharacterControllerSystem.Filter filter)
        {

        }
    }
}
