using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class SeekingExplodingItem : SeekingItem
    {
        public HitboxSettings HitboxSettings;
        public Shape2DConfig Shape;
        public int Lifetime;

        public override unsafe void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef item, ItemInstance* itemInstance)
        {
            EntityRef hitbox = HitboxSystem.SpawnHitbox(f, HitboxSettings, Shape, Lifetime, user);
            if (f.Unsafe.TryGetPointer(hitbox, out Transform2D* transformHitbox) &&
                f.Unsafe.TryGetPointer(item, out Transform2D* transformItem))
            {
                transformHitbox->Position = transformItem->Position;
            }
        }
    }
}
