using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetupMatch : DeterministicCommand
    {
        public override void Serialize(BitStream stream)
        {

        }

        public void Execute(Frame f)
        {
            Log.Debug("Match Setup!");

            f.Events.OnMatchSetup();
        }
    }
}
