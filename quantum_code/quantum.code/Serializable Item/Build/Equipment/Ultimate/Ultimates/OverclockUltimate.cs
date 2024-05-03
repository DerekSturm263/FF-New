using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class OverclockUltimate : Ultimate
    {
        public override void OnBegin(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->ApparelStatsMultiplier = new() { Agility = FP._1_50, Defense = 1, Dodge = 1, Jump = 1, Weight = 1 };
                stats->MainWeaponStatsMultiplier = new() { Damage = 2, Knockback = 2, Speed = 1 };
            }
        }

        public override void OnEnd(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->ApparelStatsMultiplier = ApparelHelper.Default;
                stats->MainWeaponStatsMultiplier = MainWeaponHelper.Default;
            }
        }
    }
}
