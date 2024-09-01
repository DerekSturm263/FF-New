using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class RandomStagePicker : StagePicker
    {
        public override IEnumerable<Team> GetInitialPickers(Frame f, IEnumerable<Team> teams) => [];
        public override IEnumerable<Team> GetAllowedPickers(Frame f, IEnumerable<Team> sortedTeams) => [];

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 0;

        public override Stage GetStageFromSelected(Frame f, IEnumerable<Stage> stages)
        {
            return stages.ElementAt(f.Global->RngSession.Next(0, stages.Count()));
        }
    }
}
