using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetBuild: DeterministicCommand
    {
        public EntityRef entity;
        public Build build;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref build);
        }

        public void Execute(Frame f)
        {
            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                StatsSystem.ApplyBuild(f, entity, stats, build);
            }
        }
    }
}
