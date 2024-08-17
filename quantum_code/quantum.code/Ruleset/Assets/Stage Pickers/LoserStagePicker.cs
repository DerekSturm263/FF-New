using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class LoserStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> teams) => [teams[teams.Count - 1]];
        public override int PlayerCountNeededToPick(Frame f, int playerCount) => 1;
    }
}
