using Photon.Deterministic;

namespace Quantum
{
    partial class RuntimePlayer
    {
        public AssetRefEntityPrototype CharacterPrototype;
        public int DeviceIndex;
        public string Name;

        partial void SerializeUserData(BitStream stream)
        {
            stream.Serialize(ref CharacterPrototype);
            stream.Serialize(ref DeviceIndex);
            stream.Serialize(ref Name);
        }
    }
}