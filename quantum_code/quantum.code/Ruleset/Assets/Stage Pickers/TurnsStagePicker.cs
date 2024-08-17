using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class TurnsStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> teams) => [];
        public override int PlayerCountNeededToPick(Frame f, int playerCount) => 1;
    }
}
