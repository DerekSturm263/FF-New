using Quantum.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class VoteStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> sortedTeams) => sortedTeams;

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => playerCount;

        public override Stage GetStageFromSelected(Frame f, QList<Stage> stages)
        {
            return stages.GroupBy(item => item).OrderByDescending(item => item.Count()).ElementAt(0).Select(item => item).ElementAt(0);
        }
    }
}
