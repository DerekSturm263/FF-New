using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class WinnerStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> teams) => [teams[0]];
        public override int PlayerCountNeededToPick(Frame f, int playerCount) => 1;
    }
}
