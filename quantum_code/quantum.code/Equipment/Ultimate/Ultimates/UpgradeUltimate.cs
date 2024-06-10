using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class UpgradeUltimate : Ultimate
    {
        public override void OnBegin(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                ApparelHelper.Multiply(stats->ApparelStatsMultiplier, new() { Agility = FP._1_10, Defense = FP._1_10, Dodge = 1, Jump = 1, Weight = 1 });
                WeaponHelper.Multiply(stats->WeaponStatsMultiplier, new() { Damage = FP._1_20, Knockback = 2, Speed = FP._1_10 });
            }
        }

        public override void OnEnd(Frame f, EntityRef user)
        {

        }
    }
}
