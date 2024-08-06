namespace Quantum
{
    public unsafe partial struct FighterIndex
    {
        public static FighterIndex Invalid = new()
        {
            Local = -1,
            Global = -1,
            GlobalNoBots = -1,
            Device = -1,
            Type = FighterType.Human
        };

        public static int GetNextGlobalIndex(Frame f)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (!f.Global->PlayerSlots[i])
                    return i;
            }

            return -1;
        }

        public static int GetNextGlobalIndexNoBots(Frame f)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (!f.Global->PlayerSlotsNoBots[i])
                    return i;
            }

            return -1;
        }

        public static int GetNextGlobalIndexNoHumans(Frame f)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (!f.Global->PlayerSlotsNoHumans[i])
                    return i;
            }

            return -1;
        }

        public static EntityRef GetPlayerFromIndex(Frame f, FighterIndex index)
        {
            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (stats.Component.Index.Equals(index))
                    return stats.Entity;
            }

            return EntityRef.None;
        }

        public static FighterIndex GetFirstFighterIndex(Frame f, System.Func<FighterIndex, bool> comparer)
        {
            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (comparer.Invoke(stats.Component.Index))
                    return stats.Component.Index;
            }

            return Invalid;
        }

        public static EntityRef GetFirstEntity(Frame f, System.Func<FighterIndex, bool> comparer)
        {
            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (comparer.Invoke(stats.Component.Index))
                    return stats.Entity;
            }

            return EntityRef.None;
        }

        public override readonly string ToString() => $"(Local: {Local}, Global: {Global}, Global No Bots {GlobalNoBots}, Device: {Device}, Type: {Type})";
    }
}
