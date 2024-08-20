using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class AnyoneStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> sortedTeams) => sortedTeams;

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
