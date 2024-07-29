using Photon.Deterministic;

namespace Quantum {
  partial class RuntimeConfig {
        public AssetRefEntityPrototype Hurtbox;
        public AssetRefEntityPrototype Hitbox;
        public LayerMask HitboxLayer;

        partial void SerializeUserData(BitStream stream)
        {
            stream.Serialize(ref Hurtbox);
            stream.Serialize(ref Hitbox);
            stream.Serialize(ref HitboxLayer);
        }
    }
}