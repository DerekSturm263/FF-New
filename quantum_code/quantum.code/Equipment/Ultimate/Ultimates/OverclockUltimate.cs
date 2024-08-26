namespace Quantum
{
    [System.Serializable]
    public unsafe partial class OverclockUltimate : Ultimate
    {
        public ApparelStats ApparelStatsMultiplier;
        public WeaponStats WeaponStatsMultiplier;

        public override void OnBegin(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out PlayerStats* playerStats))
            {
                playerStats->ApparelStatsMultiplier = ApparelStatsMultiplier;
                playerStats->WeaponStatsMultiplier = WeaponStatsMultiplier;
            }
        }

        public override void OnEnd(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out PlayerStats* playerStats))
            {
                playerStats->ApparelStatsMultiplier = ApparelHelper.Default;
                playerStats->WeaponStatsMultiplier = WeaponHelper.Default;
            }
        }
    }
}
