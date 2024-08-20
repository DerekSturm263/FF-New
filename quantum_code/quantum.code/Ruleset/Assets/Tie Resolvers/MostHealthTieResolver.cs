using Quantum.Collections;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class MostHealthTieResolver : TieResolver
    {
        public override System.Func<Team, int> ResolveTie(Frame f, QList<Team> teams)
        {
            return team =>
            {
                var players = f.ResolveList(team.Players);
                return -players.Sum(item => f.Unsafe.GetPointer<Stats>(FighterIndex.GetPlayerFromIndex(f, item))->CurrentStats.Health.AsInt);
            };
        }
    }
}
