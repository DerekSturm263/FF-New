using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class WinnerStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> sortedTeams) => [sortedTeams[0]];

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
