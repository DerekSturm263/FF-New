using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEyes : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefEyes eyes;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref eyes);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Eyes applied!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
                StatsSystem.SetEyes(f, entity, stats, eyes);
        }
    }
}
