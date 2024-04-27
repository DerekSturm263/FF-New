using Quantum.Collections;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnHitboxEvent : FrameEvent
    {
        public HitboxSettings Settings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(Settings.Prototype.Id);
            EntityRef hitboxEntity = f.Create(hitboxPrototype);

            if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitbox))
            {
                hitbox->Lifetime = EndingFrame - StartingFrame;
                hitbox->Settings = Settings;
                hitbox->Owner = entity;

                hitbox->Parent = Settings.Parent switch
                {
                    ParentType.User => entity,
                    ParentType.MainWeapon => entity,
                    ParentType.SubWeapon => entity,
                    _ => entity
                };
            }

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                QList<EntityRef> hitboxLists = f.ResolveList(stats->Hitboxes);
                hitboxLists.Add(hitboxEntity);
            }
        }
    }
}
