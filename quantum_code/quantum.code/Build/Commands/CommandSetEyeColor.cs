using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetEyeColor : DeterministicCommand
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
            Log.Debug("Eye color applied!");
        }
    }
}
