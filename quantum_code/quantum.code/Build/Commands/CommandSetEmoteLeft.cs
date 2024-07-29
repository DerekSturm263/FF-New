using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEmoteLeft : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefEmote emote;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref emote);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Emote Left applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetEmoteLeft(f, entity, stats, emote);
        }
    }
}
