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
            HitboxSystem.SpawnHitbox(f, HitboxSettings, Shape, Lifetime, user, item);

            base.OnHit(f, user, target, item, itemInstance);
        }
    }
}
