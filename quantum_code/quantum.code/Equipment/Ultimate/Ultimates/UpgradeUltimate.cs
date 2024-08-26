using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class UpgradeUltimate : Ultimate
    {
        public ApparelStats ApparelStatsMultiplier;
        public WeaponStats WeaponStatsMultiplier;

        public override void OnBegin(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out PlayerStats* stats))
            {
                stats->ApparelStatsMultiplier = ApparelHelper.Multiply(stats->ApparelStatsMultiplier, ApparelStatsMultiplier);
                stats->WeaponStatsMultiplier = WeaponHelper.Multiply(stats->WeaponStatsMultiplier, WeaponStatsMultiplier);
            }
        }

        public override void OnEnd(Frame f, EntityRef user)
        {

        }
    }
}
