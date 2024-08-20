using Quantum.Collections;
using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class TurnsStagePicker : StagePicker
    {
        public override List<Team> GetInitialPickers(Frame f, QList<Team> teams) => [teams[0]];
        public override List<Team> GetAllowedPickers(Frame f, List<Team> sortedTeams)
        {
            List<Team> team = [f.ResolveList(f.Global->Teams)[f.Global->SelectionIndex % f.Global->TotalPlayers]];
            return team;
        }

        public override int GetPlayerCountToDecide(Frame f, int playerCount) => 1;
    }
}
