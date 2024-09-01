using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnItemEvent : FrameEvent
    {
        public ItemSpawnSettings UnchargedSettings;
        public ItemSpawnSettings FullyChargedSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning projectile!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                ItemSpawnSettings settings = characterController->LerpFromAnimationHold(ItemSpawnSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                ItemSpawnSystem.SpawnParented(f, settings, entity);
            }
        }
    }
}
