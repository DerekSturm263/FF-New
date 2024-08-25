namespace Quantum
{
    public static class SystemSetup
    {
        public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig)
        {
            return [
                // pre-defined core systems
                new Core.CullingSystem2D(),
                new ChildParentLinkSystem(),

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
                new ShakeableSystem(),
                new CharacterControllerSystem(),
                new ConfirmationSystem(),
                new StatsSystem(),
                new PlayerStatsSystem(),
                new ItemSpawnSystem(),
                new ItemSystem(),
                new HurtboxSystem(),

                // animation-related systems
                new CustomAnimatorSystem(),
                new CustomAnimatorStateSystem(),
                new CustomAnimatorResetTriggersSystem()
            ];
        }
    }
}
