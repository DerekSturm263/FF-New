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
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                switch (HealingStat)
                {
                    case Stat.Health:
                        StatsSystem.ModifyHealth(f, user, stats, Amount, true);
                        break;

                    case Stat.Energy:
                        StatsSystem.ModifyEnergy(f, user, stats, Amount);
                        break;

                    case Stat.Stocks:
                        StatsSystem.ModifyStocks(f, user, stats, Amount);
                        break;
                }
            }

            base.Invoke(f, user, item, itemInstance);
        }
    }
}
