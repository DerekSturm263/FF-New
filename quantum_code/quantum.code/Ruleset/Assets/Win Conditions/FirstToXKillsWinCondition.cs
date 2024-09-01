using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class FirstToXKillsWinCondition : WinCondition
    {
        public int Count;

        public override bool IsMatchOver(Frame f, IEnumerable<Team> teams)
        {
            // Get if one team has X kills.
            bool isOneTeamWinning = teams.Any(team => team.Get(f).Sum(item => f.Unsafe.GetPointer<PlayerStats>(FighterIndex.GetPlayerFromIndex(f, item.Index))->Stats.Kills) >= Count);

            // Return true if the match would normally be over OR if 1 team hits X kills.
            return base.IsMatchOver(f, teams) || isOneTeamWinning;
        }

        public override System.Func<Team, int> SortTeams(Frame f, IEnumerable<Team> teams)
        {
            return team => -team.Get(f).Sum(item => f.Unsafe.GetPointer<PlayerStats>(FighterIndex.GetPlayerFromIndex(f, item.Index))->Stats.Kills);
        }
    }
}
