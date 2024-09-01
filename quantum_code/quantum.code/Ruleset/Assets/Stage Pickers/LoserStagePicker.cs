using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class LoserStagePicker : StagePicker
    {
        public override IEnumerable<Team> GetAllowedPickers(Frame f, IEnumerable<Team> sortedTeams) => [sortedTeams.ElementAt(sortedTeams.Count() - 1)];

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
