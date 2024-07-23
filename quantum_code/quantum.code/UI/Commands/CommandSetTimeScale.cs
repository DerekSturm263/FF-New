using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetTimeScale : DeterministicCommand
    {
        public FP scale;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref scale);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Time scale set!");

            f.Global->DeltaTime = scale;
        }
    }
}
