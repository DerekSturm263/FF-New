using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetLegwear : DeterministicCommand
    {
        public EntityRef entity;
        public Apparel legwear;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref legwear);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Legwear applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetLegwear(f, entity, stats, legwear);
        }
    }
}
