using Photon.Deterministic;
using Quantum.Collections;

namespace Quantum
{
    public unsafe class CommandMakeStageSelection : DeterministicCommand
    {
        public FighterIndex fighterIndex;
        public Stage stage;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref fighterIndex);
            stream.Serialize(ref stage);
        }

        public void Execute(Frame f)
        {
            Log.Debug($"Stage selected by {fighterIndex}!");

            if (!fighterIndex.Equals(FighterIndex.Invalid))
            {
                ++f.Global->SelectedPlayerCount;
            }

            f.ResolveList(f.Global->StagesPicked).Add(stage);

            StagePicker stagePicker = f.FindAsset<StagePicker>(f.Global->CurrentMatch.Ruleset.Stage.StagePicker.Id);
            if (f.Global->SelectedPlayerCount == stagePicker.GetPlayerCountToDecide(f, f.Global->TotalPlayers))
            {
                QList<Stage> stagesPicked = f.ResolveList(f.Global->StagesPicked);
                MatchSystem.SetStage(f, stagePicker.GetStageFromSelected(f, stagesPicked));
            }
        }
    }
}
