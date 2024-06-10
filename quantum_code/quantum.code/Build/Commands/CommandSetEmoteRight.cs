using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEmoteRight : DeterministicCommand
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
            Log.Debug("Emote Right applied!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
                StatsSystem.SetEmoteRight(f, entity, stats, emote);
        }
    }
}
