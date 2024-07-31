namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnHitboxEvent : FrameEvent
    {
        public enum ParentType
        {
            Player,
            Weapon
        }

        public ParentType Parent;
        public Shape2DConfig Shape;

        public HitboxSettings UnchargedSettings;
        public HitboxSettings FullyChargedSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning hitbox!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                HitboxSettings settings = characterController->LerpFromAnimationHold(HitboxSettings.Lerp, UnchargedSettings, FullyChargedSettings);

                if (Parent == ParentType.Player)
                {
                    HitboxSystem.SpawnHitbox(f, settings, Shape, Length, entity, entity);
                }
                else
                {
                    EntityRef parent = f.Unsafe.GetPointer<PlayerStats>(entity)->ActiveWeapon;
                    HitboxSystem.SpawnHitbox(f, settings, Shape, Length, entity, parent);
                }
            }
        }
    }
}
