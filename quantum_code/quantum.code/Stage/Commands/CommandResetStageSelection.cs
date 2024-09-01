using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandResetStageSelection : DeterministicCommand
    {
        public override void Serialize(BitStream stream)
        {

        }

        public void Execute(Frame f)
        {
            Log.Debug("Count reset!");

            f.Global->SelectedPlayerCount = 0;
            f.ResolveList(f.Global->StagesPicked).Clear();
        }
    }
}
