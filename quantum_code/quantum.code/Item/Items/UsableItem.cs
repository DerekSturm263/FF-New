using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class UsableItem : Item
    {
        public int Uses;

        public override void Invoke(Frame f, PlayerLink* user, EntityRef item, ItemInstance* itemInstance)
        {
            ++itemInstance->Uses;

            f.Events.OnItemUse(*user, *itemInstance, this);

            if (itemInstance->Uses == Uses)
            {
                f.Destroy(item);

                if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                    --itemSpawner->CurrentSpawned;

                return;
            }
        }
    }
}
