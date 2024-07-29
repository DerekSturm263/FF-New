using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetSub : DeterministicCommand
    {
        public EntityRef entity;
        public Sub sub;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref sub);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Sub applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetSub(f, entity, stats, sub);
        }
    }
}
