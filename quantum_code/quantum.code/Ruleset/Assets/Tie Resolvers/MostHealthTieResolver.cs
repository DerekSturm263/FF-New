using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class MostHealthTieResolver : TieResolver
    {
        public override System.Func<Team, int> ResolveTie(Frame f, IEnumerable<Team> teams)
        {
            return team => -team.Get(f).Sum(item => f.Unsafe.GetPointer<Stats>(FighterIndex.GetPlayerFromIndex(f, item.Index))->CurrentStats.Health.AsInt);
        }
    }
}
