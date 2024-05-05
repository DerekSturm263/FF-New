namespace Quantum {
  public static class SystemSetup {
    public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
            return [
        // pre-defined core systems
        new Core.CullingSystem2D(),
        //new Core.CullingSystem3D(),

        // physics querying
        new HitboxSystem(),
        new HitboxHurtboxQueryInjectionSystem(),

        new Core.PhysicsSystem2D(),
        //new Core.PhysicsSystem3D(),

        Core.DebugCommand.CreateSystem(),

        // new Core.NavigationSystem(),
        new Core.EntityPrototypeSystem(),
        new Core.PlayerConnectedSystem(),
        new MatchSystem(),

        // user systems go here 
        new CharacterControllerSystem(),
        new PlayerSpawnSystem(),
        new ConfirmationSystem(),
        new TimerSystem(),
        new HurtboxSystem(),
        new StatsSystem(),
        new SubWeaponSystem(),
        new ItemSpawnSystem(),
        new ItemSystem(),

        // physics resolution
        new HitboxHurtboxRetrievalSystem(),

        // animation-related systems
        new CustomAnimatorSystem(),
        new CustomAnimatorStateSystem(),
        new CustomAnimatorResetTriggersSystem()
      ];
    }
  }
}
