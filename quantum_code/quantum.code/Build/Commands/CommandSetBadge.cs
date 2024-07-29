using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetBadge : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefBadge badge;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref badge);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Badge applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetBadge(f, entity, stats, badge);
        }
    }
}
