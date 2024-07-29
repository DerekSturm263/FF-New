using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEmoteUp : DeterministicCommand
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
            Log.Debug("Emote Up applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetEmoteUp(f, entity, stats, emote);
        }
    }
}
