using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class AnyoneStagePicker : StagePicker
    {
        public override List<Team> GetAllowedPickers(Frame f, List<Team> teams) => teams;
        public override int PlayerCountNeededToPick(Frame f, int playerCount) => 1;
    }
}
