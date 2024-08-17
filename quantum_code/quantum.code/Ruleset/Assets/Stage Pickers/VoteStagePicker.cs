using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class VoteStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> teams) => teams;
        public override int PlayerCountNeededToPick(Frame f, int playerCount) => playerCount;

        public override Stage ChooseStageFromPicked(Frame f, List<Stage> stages)
        {
            return stages.GroupBy(item => item).OrderByDescending(item => item.Count()).ElementAt(0).Select(item => item).ElementAt(0);
        }
    }
}
