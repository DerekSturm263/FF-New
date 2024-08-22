using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class AnyoneStagePicker : StagePicker
    {
        public override IEnumerable<Team> GetAllowedPickers(Frame f, IEnumerable<Team> sortedTeams) => sortedTeams;

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
