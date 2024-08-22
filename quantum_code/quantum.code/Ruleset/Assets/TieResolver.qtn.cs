using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class TieResolver : InfoAsset
    {
        public abstract System.Func<Team, int> ResolveTie(Frame f, IEnumerable<Team> teams);
    }
}
