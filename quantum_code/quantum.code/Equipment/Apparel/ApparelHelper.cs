using Photon.Deterministic;

namespace Quantum
{
    public static class ApparelHelper
    {
        private static ApparelStats Add(ApparelStats lhs, ApparelStats rhs)
        {
            return new()
            {
                Agility = lhs.Agility + rhs.Agility,
                Jump = lhs.Jump + rhs.Jump,
                Weight = lhs.Weight + rhs.Weight,
                Dodge = lhs.Dodge + rhs.Dodge,
                Defense = lhs.Defense + rhs.Defense
            };
        }

        public static ApparelStats Multiply(ApparelStats lhs, ApparelStats rhs)
        {
            return new()
            {
                Agility = lhs.Agility * rhs.Agility,
                Jump = lhs.Jump * rhs.Jump,
                Weight = lhs.Weight * rhs.Weight,
                Dodge = lhs.Dodge * rhs.Dodge,
                Defense = lhs.Defense * rhs.Defense
            };
        }

        public static unsafe ApparelStats FromStats(Frame f, Stats* stats)
        {
            ApparelStats apparelStats = Default;

            apparelStats = Add(FromApparel(f, stats->Build.Equipment.Outfit.Headgear), apparelStats);
            apparelStats = Add(FromApparel(f, stats->Build.Equipment.Outfit.Clothing), apparelStats);
            apparelStats = Add(FromApparel(f, stats->Build.Equipment.Outfit.Legwear), apparelStats);

            return Multiply(apparelStats, stats->ApparelStatsMultiplier);
        }

        private static ApparelStats FromApparel(Frame f, Apparel apparel)
        {
            ApparelStats result = default;

            if (f.TryFindAsset(apparel.Modifiers.Modifier1.Id, out ApparelModifier modifier1))
                result = Add(result, modifier1.Stats);
            
            if (f.TryFindAsset(apparel.Modifiers.Modifier2.Id, out ApparelModifier modifier2))
                result = Add(result, modifier2.Stats);
            
            if (f.TryFindAsset(apparel.Modifiers.Modifier3.Id, out ApparelModifier modifier3))
                result = Add(result, modifier3.Stats);

            return result;
        }

        public static ApparelStats Default = new()
        {
            Agility = 1,
            Jump = 1,
            Weight = 1,
            Dodge = 1,
            Defense = 1
        };
    }
}
