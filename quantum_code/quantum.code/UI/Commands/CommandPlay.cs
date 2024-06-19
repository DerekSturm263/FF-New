using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandPlay : DeterministicCommand
    {
        public override void Serialize(BitStream stream)
        {

        }

        public void Execute(Frame f)
        {
            Log.Debug("Game played!");

            f.Global->DeltaTime = FP._1 / f.UpdateRate;
        }
    }
}
