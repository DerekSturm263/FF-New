using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEmoteLeftMessage : DeterministicCommand
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
            Log.Debug("Emote Left message applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetEmoteRightMessage(f, entity, stats, message);
        }
    }
}
