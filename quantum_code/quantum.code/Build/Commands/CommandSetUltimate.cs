using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetUltimate : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefUltimate ultimate;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref ultimate);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Ultimate applied!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
                StatsSystem.SetUltimate(f, entity, stats, ultimate);
        }
    }
}
