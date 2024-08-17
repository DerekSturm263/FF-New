using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class RandomStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> teams) => [];
        public override int PlayerCountNeededToPick(Frame f, int playerCount) => 0;

        public override Stage ChooseStageFromPicked(Frame f, List<Stage> stages)
        {
            return stages[f.Global->RngSession.Next(0, stages.Count)];
        }
    }
}
