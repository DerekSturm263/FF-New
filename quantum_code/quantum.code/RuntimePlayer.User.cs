using Photon.Deterministic;

namespace Quantum
{
    partial class RuntimePlayer
    {
        public AssetRefEntityPrototype CharacterPrototype;

        public string Name;
        public FighterIndex Index;
        public Build Build;
        public bool IsRealBattle;

        partial void SerializeUserData(BitStream stream)
        {
            stream.Serialize(ref CharacterPrototype);
            stream.Serialize(ref Name);
            stream.Serialize(ref Index);
            stream.Serialize(ref Build);
            stream.Serialize(ref IsRealBattle);
        }
    }
}