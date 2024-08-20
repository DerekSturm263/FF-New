﻿using Quantum.Collections;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class MostKillsWinCondition : WinCondition
    {
        public override System.Func<Team, int> SortTeams(Frame f, QList<Team> teams)
        {
            return team =>
            {
                var players = f.ResolveList(team.Players);
                return -players.Sum(item => f.Unsafe.GetPointer<PlayerStats>(FighterIndex.GetPlayerFromIndex(f, item))->Stats.Kills);
            };
        }
    }
}
