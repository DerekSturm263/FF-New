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
                f.Events.OnItemUse(user, item, this, transform->Position);

            if (itemInstance->Uses == Uses)
            {
                ItemSpawnSystem.Despawn(f, item);
            }
        }
    }
}
