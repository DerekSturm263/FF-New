using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class HealingItem : UsableItem
    {
        public enum Stat
        {
            Health,
            Energy,
            Stocks
        }

        public Stat HealingStat;
        public int Amount;

        public override void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats) && f.Unsafe.TryGetPointer(user, out PlayerLink* playerLink))
            {
                switch (HealingStat)
                {
                    case Stat.Health:
                        StatsSystem.ModifyHealth(f, playerLink, stats, Amount);
                        break;

                    case Stat.Energy:
                        StatsSystem.ModifyEnergy(f, playerLink, stats, Amount);
                        break;

                    case Stat.Stocks:
                        StatsSystem.ModifyStocks(f, playerLink, stats, Amount);
                        break;
                }
            }

            base.Invoke(f, user, item, itemInstance);
        }
    }
}
