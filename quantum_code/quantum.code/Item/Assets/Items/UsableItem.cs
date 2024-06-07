using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class UsableItem : Item
    {
        public int Uses;

        public override void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            ++itemInstance->Uses;

            if (f.Unsafe.TryGetPointer(item, out Transform2D* transform))
                f.Events.OnItemUse(user, f.Get<Stats>(user).PlayerIndex, item, this, transform->Position);

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
