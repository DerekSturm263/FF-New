using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetAvatarColor : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefColorPreset color;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref color);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Avatar color applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetAvatarColor(f, entity, stats, color);
        }
    }
}
