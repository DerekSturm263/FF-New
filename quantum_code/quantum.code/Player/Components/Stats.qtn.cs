namespace Quantum
{
    public unsafe partial struct Stats
    {
        public FighterIndex GetIndex(Frame f, EntityRef entity)
        {
            return new()
            {
                Local = GetLocalIndex(f, entity),
                Global = GetGlobalIndex(f, entity),
                Internal = GetInternalIndex(f, entity)
            };
        }

        private readonly int GetLocalIndex(Frame f, EntityRef entity)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (f.Global->AllPlayers[DeviceIndex * 4 + i] == entity)
                    return i;
            }

            return -1;
        }

        private readonly int GetGlobalIndex(Frame f, EntityRef entity)
        {
            int j = -1;
            for (int i = 0; i < f.Global->AllPlayers.Length; ++i)
            {
                if (f.Global->AllPlayers[i].IsValid)
                    ++j;

                if (f.Global->AllPlayers[i] == entity)
                    return j;
            }

            return -1;
        }

        private readonly int GetInternalIndex(Frame f, EntityRef entity)
        {
            for (int i = 0; i < f.Global->AllPlayers.Length; ++i)
            {
                if (f.Global->AllPlayers[i] == entity)
                    return i;
            }

            return -1;
        }
    }
}
