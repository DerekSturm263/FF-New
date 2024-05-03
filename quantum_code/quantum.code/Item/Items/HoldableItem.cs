using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class HoldableItem : Item
    {
        public enum Handedness
        {
            OneHanded,
            TwoHanded
        }

        public Handedness HandednessType;
        public bool DestroySelfOnHit;

        public override void Invoke(Frame f, PlayerLink* user, EntityRef item, ItemInstance* itemInstance)
        {
            ItemSystem.PickUp(f, user->Entity, item);
        }

        public virtual void OnHit(Frame f, PlayerLink* user, EntityRef target, EntityRef item, ItemInstance* itemInstance)
        {
            if (DestroySelfOnHit)
            {
                f.Destroy(item);

                if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                    --itemSpawner->CurrentSpawned;
            }

            f.Events.OnItemUse(*user, *itemInstance, this);
        }
    }
}
