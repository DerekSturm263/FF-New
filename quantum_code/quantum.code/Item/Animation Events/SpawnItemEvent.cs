using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnItemEvent : FrameEvent
    {
        public ItemSpawnSettings Settings;
        public ItemSpawnSettings MaxHoldSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning projectile!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                ItemSpawnSettings settings;

                if (characterController->MaxHoldAnimationFrameTime > 0)
                    settings = ItemSpawnSettings.Lerp(Settings, MaxHoldSettings, (FP)characterController->HeldAnimationFrameTime / characterController->MaxHoldAnimationFrameTime);
                else
                    settings = Settings;

                ItemSpawnSystem.SpawnWithOwner(f, settings, entity);
            }
        }
    }
}
