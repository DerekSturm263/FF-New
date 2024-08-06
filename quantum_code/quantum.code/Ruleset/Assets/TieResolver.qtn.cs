using Quantum.Collections;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class TieResolver : InfoAsset
    {
        public abstract System.Func<Team, int> ResolveTie(Frame f, QList<Team> teams);
    }
}
