using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class Item : InfoAsset
    {
        public AssetRefEntityPrototype Prototype;

        public abstract void Invoke(Frame f, PlayerLink* user, EntityRef item, ItemInstance* itemInstance);
    }
}
