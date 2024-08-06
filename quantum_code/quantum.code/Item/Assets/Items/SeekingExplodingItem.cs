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
            HitboxSystem.SpawnHitbox(f, HitboxSettings, Shape, Lifetime, user, item);
        }
    }
}
