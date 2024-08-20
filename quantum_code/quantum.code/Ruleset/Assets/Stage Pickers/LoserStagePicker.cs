using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class LoserStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> sortedTeams) => [sortedTeams[sortedTeams.Count - 1]];

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
