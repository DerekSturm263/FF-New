using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class StagePicker : InfoAsset
    {
        public abstract List<Team> GetAllowedPickers(Frame f, List<Team> teams);
        public abstract int PlayerCountNeededToPick(Frame f, int playerCount);

        public virtual Stage ChooseStageFromPicked(Frame f, List<Stage> stages) => stages[0];
    }
}
