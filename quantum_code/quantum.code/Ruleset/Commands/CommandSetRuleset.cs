using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe class CommandSetRuleset : DeterministicCommand
    {
        public Ruleset ruleset;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref ruleset);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Ruleset applied!");

            int index = -1;

            if (f.Global->LastSelector == -1 && (ruleset.Stage.StagePicker != StagePickerType.Anyone || ruleset.Stage.StagePicker != StagePickerType.Vote))
            {
                index = 0;
            }
            else
            {
                Team selectingTeam = GetSelectingTeam(f.Global->LastSelector, f.Global->RngSession, f.Global->Results, ruleset);

                if (!selectingTeam.Equals(default(Team)))
                {
                    var players = f.ResolveList(selectingTeam.Players);

                    if (f.Unsafe.TryGetPointer(players[0], out Stats* stats))
                    {
                        index = stats->GlobalIndex;
                    }
                }
            }

            f.Events.OnStageSetSelector(index, ruleset.Stage.StagePicker == StagePickerType.Vote);
            f.Global->LastSelector = index;

            MatchSystem.SetRuleset(f, ruleset);
        }

        private Team GetSelectingTeam(int lastSelector, RNGSession rng, MatchResults results, Ruleset ruleset)
        {
            return ruleset.Stage.StagePicker switch
            {
                StagePickerType.Turns => ArrayHelper.Get(results.Teams, (lastSelector + 1) % results.Count),
                StagePickerType.Loser => ArrayHelper.Get(results.Teams, results.Count - 1),
                StagePickerType.Winner => ArrayHelper.Get(results.Teams, 0),
                StagePickerType.Random => ArrayHelper.Get(results.Teams, rng.Next(0, results.Count)),
                _ => default,
            };
        }
    }
}
