namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ExplodingItem : HoldableItem
    {
        public HitboxSettings HitboxSettings;
        public Shape2DConfig Shape;

        public int HitboxLifetime;

        public override unsafe bool OnHit(Frame f, ref CharacterControllerSystem.Filter user, EntityRef target, ref ItemSystem.Filter filter)
        {
            HitboxSystem.SpawnHitbox(f, HitboxSettings, Shape.CreateShape(f), HitboxLifetime, ref user, [filter.Transform->Position], EntityRef.None, null);

            return base.OnHit(f, ref user, target, ref filter);
        }
    }
}
