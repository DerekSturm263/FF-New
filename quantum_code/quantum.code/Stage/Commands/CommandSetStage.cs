using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetStage : DeterministicCommand
    {
        public Stage stage;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref stage);
        }

        public void Execute(Frame f)
        {
            MatchSystem.SetStage(f, stage);
        }
    }
}
