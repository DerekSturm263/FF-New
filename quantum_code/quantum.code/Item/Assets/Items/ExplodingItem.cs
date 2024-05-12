namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ExplodingItem : HoldableItem
    {
        public HitboxSettings HitboxSettings;
        public int Lifetime;

        public override unsafe void OnHit(Frame f, PlayerLink* user, EntityRef target, EntityRef item, ItemInstance* itemInstance)
        {
            EntityRef hitbox = HitboxSystem.SpawnHitbox(f, HitboxSettings, Lifetime, user->Entity);
            if (f.Unsafe.TryGetPointer(hitbox, out Transform2D* transformHitbox) &&
                f.Unsafe.TryGetPointer(item, out Transform2D* transformItem))
            {
                transformHitbox->Position = transformItem->Position;
            }

            base.OnHit(f, user, target, item, itemInstance);
        }
    }
}
