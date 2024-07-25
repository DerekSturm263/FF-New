namespace Quantum
{
    public static class SystemSetup
    {
        public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig)
        {
            return [
                // pre-defined core systems
                new Core.CullingSystem2D(),

                // physics querying
                new HitboxSystem(),
                new HitboxHurtboxQueryInjectionSystem(),

                new Core.PhysicsSystem2D(),

                Core.DebugCommand.CreateSystem(),

                // command systems go here
                new CommandSystem(),

                // new Core.NavigationSystem(),
                new Core.EntityPrototypeSystem(),
                new Core.PlayerConnectedSystem(),

                // user systems go here 

                // physics resolution
                new HitboxHurtboxRetrievalSystem(),

                new MatchSystem(),
                new TimerSystem(),
                new PlayerSpawnSystem(),
                new CharacterControllerSystem(),
                new ConfirmationSystem(),
                new StatsSystem(),
                new ItemSpawnSystem(),
                new ItemSystem(),

                // animation-related systems
                new CustomAnimatorSystem(),
                new CustomAnimatorStateSystem(),
                new CustomAnimatorResetTriggersSystem(),

                new HurtboxSystem()
            ];
        }
    }
}
