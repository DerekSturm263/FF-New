namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnItemEvent : FrameEvent
    {
        public ItemSpawnSettings UnchargedSettings;
        public ItemSpawnSettings FullyChargedSettings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Spawning projectile!");

            ItemSpawnSettings settings = filter.CharacterController->LerpFromAnimationHold(ItemSpawnSettings.Lerp, UnchargedSettings, FullyChargedSettings);
            ItemSpawnSystem.SpawnParented(f, settings, filter.Entity);
        }
    }
}
