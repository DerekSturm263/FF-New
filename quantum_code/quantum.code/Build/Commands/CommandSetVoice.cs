using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetVoice : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefVoice voice;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref voice);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Voice applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetVoice(f, entity, stats, voice);
        }
    }
}
