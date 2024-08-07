using Quantum.Collections;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class FirstToXKillsWinCondition : WinCondition
    {
        public int Count;

        public override bool IsMatchOver(Frame f, QList<Team> teams)
        {
            // Get if one team has X kills.
            bool isOneTeamWinning = teams.Any(team => {
                var players = f.ResolveList(team.Players);
                return players.Sum(item => f.Unsafe.GetPointer<PlayerStats>(item)->Stats.Kills) >= Count;
            });

            // Return true if the match would normally be over OR if 1 team hits X kills.
            return base.IsMatchOver(f, teams) || isOneTeamWinning;
        }

        public override System.Func<Team, int> SortTeams(Frame f, QList<Team> teams)
        {
            return team =>
            {
                var players = f.ResolveList(team.Players);
                return -players.Sum(item => f.Unsafe.GetPointer<PlayerStats>(item)->Stats.Kills);
            };
        }
    }
}
