namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ProjectileSpawningItem : UsableItem
    {
        public int Amount;

        public override void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {

            }

            base.Invoke(f, user, item, itemInstance);
        }
    }
}
