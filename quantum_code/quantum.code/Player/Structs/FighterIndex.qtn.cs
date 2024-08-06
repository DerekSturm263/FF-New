﻿using System;
using System.Collections.Generic;

namespace Quantum
{
    public unsafe partial struct FighterIndex : IComparable<FighterIndex>
    {
        public static FighterIndex Invalid = new()
        {
            Local = -1,
            Global = -1,
            GlobalNoBots = -1,
            GlobalNoHumans = -1,
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
                if (stats.Component.Index.CompareTo(index) == 0)
                    return stats.Entity;
            }

            return EntityRef.None;
        }

        public static FighterIndex GetFirstFighterIndex(Frame f, Func<FighterIndex, bool> comparer)
        {
            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (comparer.Invoke(stats.Component.Index))
                    return stats.Component.Index;
            }

            return Invalid;
        }

        public static IEnumerable<FighterIndex> GetFighterIndexList(Frame f, Func<FighterIndex, bool> comparer)
        {
            List<FighterIndex> result = [];

            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (comparer.Invoke(stats.Component.Index))
                    result.Add(stats.Component.Index);
            }

            return result;
        }

        public static EntityRef GetFirstEntity(Frame f, Func<FighterIndex, bool> comparer)
        {
            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (comparer.Invoke(stats.Component.Index))
                    return stats.Entity;
            }

            return EntityRef.None;
        }

        public static IEnumerable<EntityRef> GetEntityList(Frame f, Func<FighterIndex, bool> comparer)
        {
            List<EntityRef> result = [];

            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (comparer.Invoke(stats.Component.Index))
                    result.Add(stats.Entity);
            }

            return result;
        }

        public override readonly string ToString() => $"(Local: {Local}, Global: {Global}, Global No Bots {GlobalNoBots}, Global No Humans {GlobalNoHumans}, Device: {Device}, Type: {Type})";

        public readonly int CompareTo(FighterIndex other)
        {
            return (Local - other.Local) +
                   (Global - other.Global) +
                   (GlobalNoBots - other.GlobalNoBots) +
                   (GlobalNoHumans - other.GlobalNoHumans) +
                   (Device - other.Device) +
                   (Type - other.Type);
        }
    }
}
