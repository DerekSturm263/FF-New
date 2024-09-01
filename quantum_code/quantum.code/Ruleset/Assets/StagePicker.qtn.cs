using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class StagePicker : InfoAsset
    {
        public string SelectingMessage;
        public string FallbackMessage;

        public virtual IEnumerable<Team> GetInitialPickers(Frame f, IEnumerable<Team> teams) => teams.ToList();
        public abstract IEnumerable<Team> GetAllowedPickers(Frame f, IEnumerable<Team> sortedTeams);

        public abstract int GetPlayerCountToDecide(Frame f, int playerCount);

        public virtual Stage GetStageFromSelected(Frame f, IEnumerable<Stage> stages) => stages.ElementAt(0);
    }
}
