using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class VoteStagePicker : StagePicker
    {
        public override IEnumerable<Team> GetAllowedPickers(Frame f, IEnumerable<Team> sortedTeams) => sortedTeams;

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => playerCount;

        public override Stage GetStageFromSelected(Frame f, IEnumerable<Stage> stages)
        {
            return stages.GroupBy(item => item).OrderByDescending(item => item.Count()).ElementAt(0).Select(item => item).ElementAt(0);
        }
    }
}
