using Quantum.Collections;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class BestKillToDeathRatioWinCondition : WinCondition
    {
        public override unsafe System.Func<Team, int> SortTeams(Frame f, QList<Team> teams)
        {
            return team =>
            {
                var players = f.ResolveList(team.Players);
                return players.Sum(item => f.Unsafe.GetPointer<Stats>(item)->Deaths - f.Unsafe.GetPointer<Stats>(item)->Kills);
            };
        }
    }
}
