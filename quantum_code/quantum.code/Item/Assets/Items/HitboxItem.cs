using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class HitboxItem : UpdateableItem
    {
        public HitboxSettings HitboxSettings;
        public Shape2DConfig Shape;

        public override unsafe void OnStart(Frame f, EntityRef user, ref ItemSystem.Filter filter)
        {
            base.OnStart(f, user, ref filter);

            filter.ItemInstance->IsActive = true;
            filter.ItemInstance->Hitbox = HitboxSystem.SpawnHitbox(f, HitboxSettings, Shape.CreateShape(f), Lifetime, user, [FPVector2.Zero], filter.Entity, null);
        }

        public override unsafe void OnUpdate(Frame f, EntityRef user, ref ItemSystem.Filter filter)
        {
            if (filter.ItemInstance->ActiveTime >= Lifetime)
                ItemSpawnSystem.Despawn(f, filter.Entity);
        }

        public override unsafe void OnExit(Frame f, EntityRef user, ref ItemSystem.Filter filter) { }
    }
}
