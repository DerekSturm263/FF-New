using Quantum.Collections;
using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class RandomStagePicker : StagePicker
    {
        public override List<Team> GetInitialPickers(Frame f, QList<Team> teams) => [];
        public override List<Team> GetAllowedPickers(Frame f, List<Team> sortedTeams) => [];

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 0;

        public override Stage GetStageFromSelected(Frame f, QList<Stage> stages)
        {
            return stages[f.Global->RngSession.Next(0, stages.Count)];
        }
    }
}
