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

        public override void Invoke(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter)
        {
            switch (HealingStat)
            {
                case Stat.Health:
                    StatsSystem.ModifyHealth(f, user.Entity, user.Stats, Amount, true);
                    break;

                case Stat.Energy:
                    StatsSystem.ModifyEnergy(f, user.Entity, user.Stats, Amount);
                    break;

                case Stat.Stocks:
                    StatsSystem.ModifyStocks(f, user.Entity, user.Stats, Amount);
                    break;
            }

            base.Invoke(f, ref user, ref filter);
        }
    }
}
