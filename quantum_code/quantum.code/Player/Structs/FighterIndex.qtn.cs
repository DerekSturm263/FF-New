using Quantum.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    public unsafe partial struct FighterIndex
    {
        public static FighterIndex Invalid = new()
        {
            Local = -1,
            Global = -1,
            GlobalNoBots = -1,
            GlobalNoHumans = -1,
            Team = -1,
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

        public readonly ColorRGBA GetLightColor(Frame f) => ArrayHelper.All(f.RuntimeConfig.TeamColors)[Team];
        public readonly ColorRGBA GetDarkColor(Frame f) => ArrayHelper.All(f.RuntimeConfig.DarkTeamColors)[Team];

        private static IEnumerable<Team> GetAllTeams(Frame f)
        {
            // Create a new list of players.
            List<PlayerNameIndex> players = [];

            // Add each player to the list of players
            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                players.Add(new() { Index = stats.Component.Index, Name = stats.Component.Name });
            }

            // Return the list grouped by their teams.
            return players.GroupBy(item => item.Index.Team).Select(item =>
            {
                Team team = new();
                team.Set(item);

                return team;
            });
        }

        public static void UpdateGlobalList(Frame f)
        {
            var teams = f.ResolveList(f.Global->Teams);
            teams.Clear();

            foreach (var team in GetAllTeams(f))
            {
                teams.Add(team);
            }
        }

        public override readonly string ToString() => $"(Local: {Local}, Global: {Global}, Global No Bots {GlobalNoBots}, Global No Humans {GlobalNoHumans}, Team {Team}, Device: {Device}, Type: {Type})";

        public override readonly bool Equals(object obj)
        {
            if (obj is not FighterIndex)
                return false;

            FighterIndex objF = (FighterIndex)obj;
            return objF.Local == Local && objF.Global == Global && objF.GlobalNoBots == GlobalNoBots && objF.GlobalNoHumans == GlobalNoHumans && objF.Device == Device && objF.Type == Type;
        }
    }
}
