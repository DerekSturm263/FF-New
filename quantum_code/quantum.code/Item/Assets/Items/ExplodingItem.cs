namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ExplodingItem : HoldableItem
    {
        public HitboxSettings HitboxSettings;
        public Shape2DConfig Shape;
        public int Lifetime;

        public override unsafe void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef item, ItemInstance* itemInstance)
        {
            if (f.Unsafe.TryGetPointer(item, out Transform2D* transform))
                HitboxSystem.SpawnHitbox(f, HitboxSettings, Shape.CreateShape(f), Lifetime, user, [transform->Position]);

            base.OnHit(f, user, target, item, itemInstance);
        }
    }
}
