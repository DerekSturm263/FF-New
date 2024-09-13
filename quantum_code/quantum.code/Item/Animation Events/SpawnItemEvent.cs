namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnItemEvent : FrameEvent
    {
        public Input.Buttons Button;

        public ItemSpawnSettings UnchargedSettings;
        public ItemSpawnSettings FullyChargedSettings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Spawning projectile!");

            if (Button == 0)
            {
                ItemSpawnSettings settings = filter.CharacterController->LerpFromAnimationHold(ItemSpawnSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                ItemSpawnSystem.SpawnParented(f, settings, filter.Entity);
            }

            filter.CharacterController->PressedButton = false;
        }

        public override void Update(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame, int elapsedFrames)
        {
            Log.Debug("Updating projectile!");

            if (!filter.CharacterController->PressedButton && filter.CharacterController->WasPressedThisFrame(input, Button))
            {
                filter.CharacterController->PressedButton = true;

                ItemSpawnSettings settings = filter.CharacterController->LerpFromAnimationHold(ItemSpawnSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                ItemSpawnSystem.SpawnParented(f, settings, filter.Entity);
            }
        }

        public override void End(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Cleaning up projectile!");

            filter.CharacterController->PressedButton = false;
        }
    }
}
