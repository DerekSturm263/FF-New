using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetAvatar : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefFFAvatar avatar;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref avatar);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Avatar applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetAvatar(f, entity, stats, avatar);
        }
    }
}
