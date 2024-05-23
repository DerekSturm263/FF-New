using Quantum.Collections;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class WinCondition : InfoAsset
    {
        public virtual bool IsMatchOver(Frame f, QList<Team> teams)
        {
            // Get the current match instance.
            MatchInstance* matchInstance = f.Unsafe.GetPointerSingleton<MatchInstance>();

            // Get if the match timer hits 0.
            bool isMatchOver = matchInstance->IsTimerOver;
            if (matchInstance->Match.Ruleset.Players.StockCount == -1)
                return isMatchOver;

            // Get if 1 or fewer teams have any players left alive.
            bool isOneTeamLeft = teams.Count(team => {
                var players = f.ResolveList(team.Players);
                return players.Any(item => f.Unsafe.GetPointer<Stats>(item.Entity)->CurrentStocks > 0);
            }) < 2;

            // Return true (match is over) if the match timer hits 0 OR if 1 or fewer teams have any players left alive.
            return isMatchOver || isOneTeamLeft;
        }
        
        public abstract System.Func<Team, int> SortTeams(Frame f, QList<Team> teams);
    }
}
