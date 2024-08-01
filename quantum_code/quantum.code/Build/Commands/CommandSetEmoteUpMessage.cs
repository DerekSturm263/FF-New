using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEmoteUpMessage : DeterministicCommand
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
            Log.Debug("Emote Up message applied!");
        }
    }
}
