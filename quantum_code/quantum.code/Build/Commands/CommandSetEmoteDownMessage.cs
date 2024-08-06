using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEmoteDownMessage : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefMessagePreset message;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref message);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Emote Down message applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetEmoteDownMessage(f, entity, stats, message);
        }
    }
}
