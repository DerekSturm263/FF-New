namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class Item : InfoAsset
    {
        public AssetRefEntityPrototype Prototype;

        public ItemCollisionType DestroyOnAnyHit;
        public ItemCollisionType DestroyOnHitAfterMinCollisions;

        public bool DestroySelfOnHit;

        public bool CanInteractWithOwner;
        public bool AlignDirectionToVelocity;

        public abstract void Invoke(Frame f, EntityRef user, ref ItemSystem.Filter filter);
        
        public virtual bool OnHit(Frame f, EntityRef user, EntityRef target, ref ItemSystem.Filter filter)
        {
            f.Events.OnItemUse(user, filter.Entity, this, filter.Transform->Position);
            return DestroySelfOnHit;
        }
    }
}
