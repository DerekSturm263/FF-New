using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class WinnerStagePicker : StagePicker
    {
        public override IEnumerable<Team> GetAllowedPickers(Frame f, IEnumerable<Team> sortedTeams) => [sortedTeams.ElementAt(0)];

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
