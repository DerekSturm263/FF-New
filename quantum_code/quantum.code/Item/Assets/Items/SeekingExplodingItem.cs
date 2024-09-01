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
            if (f.Unsafe.TryGetPointer(item, out Transform2D* transform))
                HitboxSystem.SpawnHitbox(f, HitboxSettings, Shape.CreateShape(f), Lifetime, user, [transform->Position], false);

            base.OnHit(f, user, target, item, itemInstance);
        }
    }
}
