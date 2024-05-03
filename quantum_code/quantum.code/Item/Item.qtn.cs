using Photon.Deterministic;

namespace Quantum
{
    public abstract unsafe partial class Item
    {
        public abstract void Invoke(Frame f, PlayerLink* user, EntityRef item, ItemInstance* itemInstance);
    }
}
