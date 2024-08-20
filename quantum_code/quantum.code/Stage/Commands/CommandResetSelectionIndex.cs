using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandResetSelectionIndex : DeterministicCommand
    {
        public override void Serialize(BitStream stream)
        {

        }

        public void Execute(Frame f)
        {
            Log.Debug("Selection index reset!");

            f.Global->SelectionIndex = 0;
        }
    }
}
