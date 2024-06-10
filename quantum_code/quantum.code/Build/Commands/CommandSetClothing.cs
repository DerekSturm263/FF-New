using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetClothing : DeterministicCommand
    {
        public EntityRef entity;
        public Apparel clothing;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref clothing);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Clothing applied!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
                StatsSystem.SetClothing(f, entity, stats, clothing);
        }
    }
}
