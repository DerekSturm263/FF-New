using Photon.Deterministic;

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
            MatchSystem.SetRuleset(f, ruleset);
        }
    }
}
