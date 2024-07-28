using Photon.Deterministic;
using Quantum.Types;
using System.ComponentModel;

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

            foreach (var stats in f.GetComponentIterator<Stats>())
            {
                f.Events.OnPlayerModifyHealth(stats.Entity, stats.Component.Index, stats.Component.CurrentHealth, stats.Component.CurrentHealth, ruleset.Players.MaxHealth);
                f.Events.OnPlayerModifyEnergy(stats.Entity, stats.Component.Index, stats.Component.CurrentEnergy, stats.Component.CurrentEnergy, ruleset.Players.MaxEnergy);
                f.Events.OnPlayerModifyStocks(stats.Entity, stats.Component.Index, stats.Component.CurrentStocks, stats.Component.CurrentStocks, ruleset.Players.StockCount);
            }

            FighterIndex index = FighterIndex.Invalid;

            if (!f.Global->LastSelector.Equals(FighterIndex.Invalid) && (ruleset.Stage.StagePicker != StagePickerType.Anyone || ruleset.Stage.StagePicker != StagePickerType.Vote))
            {
                index = FighterIndex.GetFirstFighterIndex(f, item => item.GlobalNoBots == 0);
            }
            else
            {
                Team selectingTeam = GetSelectingTeam(f.Global->LastSelector, f.Global->RngSession, f.Global->Results, ruleset);

                if (!selectingTeam.Equals(default(Team)))
                {
                    var players = f.ResolveList(selectingTeam.Players);

                    if (f.Unsafe.TryGetPointer(players[0], out Stats* stats))
                    {
                        index = stats->Index;
                    }
                }
            }

            f.Events.OnStageSetSelector(index, ruleset.Stage.StagePicker == StagePickerType.Vote);
            f.Global->LastSelector = index;

            MatchSystem.SetRuleset(f, ruleset);
        }

        private Team GetSelectingTeam(FighterIndex lastSelector, RNGSession rng, MatchResults results, Ruleset ruleset)
        {
            return ruleset.Stage.StagePicker switch
            {
                StagePickerType.Turns => ArrayHelper.Get(results.Teams, (lastSelector.Global + 1) % results.Count),
                StagePickerType.Loser => ArrayHelper.Get(results.Teams, results.Count - 1),
                StagePickerType.Winner => ArrayHelper.Get(results.Teams, 0),
                StagePickerType.Random => ArrayHelper.Get(results.Teams, rng.Next(0, results.Count)),
                _ => default,
            };
        }
    }
}
