using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class TurnsStagePicker : StagePicker
    {
        public override IEnumerable<Team> GetInitialPickers(Frame f, IEnumerable<Team> teams) => [teams.ElementAt(0)];
        public override IEnumerable<Team> GetAllowedPickers(Frame f, IEnumerable<Team> sortedTeams)
        {
            return [FighterIndex.GetAllTeams(f).ElementAt(f.Global->SelectionIndex % f.Global->TotalPlayers)];
        }

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
