using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class LeastDeathsTieResolver : TieResolver
    {
        public override System.Func<Team, int> ResolveTie(Frame f, IEnumerable<Team> teams)
        {
            return team => team.Get(f).Sum(item => f.Unsafe.GetPointer<PlayerStats>(FighterIndex.GetPlayerFromIndex(f, item))->Stats.Deaths);
        }
    }
}
