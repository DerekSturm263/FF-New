using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandPause : DeterministicCommand
    {
        public override void Serialize(BitStream stream)
        {

        }

        public void Execute(Frame f)
        {
            Log.Debug("Game paused!");

            f.Global->DeltaTime = 0;
        }
    }
}
