namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SetWeaponHitboxEvent : FrameEvent
    {
        public HitboxSettings Settings;
        public HitboxSettings MaxHoldSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Enabling hitbox!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                HitboxSettings settings = characterController->LerpFromAnimationHold(HitboxSettings.Lerp, Settings, MaxHoldSettings);
                HitboxSystem.SetStaticHitboxEnabled(f, settings, entity, true);
            }
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Disabling hitbox!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                HitboxSettings settings = characterController->LerpFromAnimationHold(HitboxSettings.Lerp, Settings, MaxHoldSettings);
                HitboxSystem.SetStaticHitboxEnabled(f, settings, entity, false);
            }
        }
    }
}
