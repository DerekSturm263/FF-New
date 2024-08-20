using Quantum.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class StagePicker : InfoAsset
    {
        public string SelectingMessage;
        public string FallbackMessage;

        public virtual List<Team> GetInitialPickers(Frame f, QList<Team> teams) => teams.ToList();
        public abstract List<Team> GetAllowedPickers(Frame f, List<Team> sortedTeams);

        public abstract int GetPlayerCountToDecide(Frame f, int playerCount);

        public virtual Stage GetStageFromSelected(Frame f, QList<Stage> stages) => stages[0];
    }
}
