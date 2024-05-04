namespace Quantum {
  public static class SystemSetup {
    public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
            return [
        // pre-defined core systems
        new Core.CullingSystem2D(),
        //new Core.CullingSystem3D(),

        // physics querying
        new HitboxSystem(),
        new Collision.HitboxHurtboxQueryInjectionSystem(),

        new Core.PhysicsSystem2D(),
        //new Core.PhysicsSystem3D(),

        Core.DebugCommand.CreateSystem(),

        // new Core.NavigationSystem(),
        new Core.EntityPrototypeSystem(),
        new Core.PlayerConnectedSystem(),

        // user systems go here 
        new Movement.PlayerStateSystem(),
        new Multiplayer.PlayerSpawnSystem(),
        new Multiplayer.ConfirmationSystem(),
        new Timing.TimerSystem(),
        new HurtboxSystem(),
        new StatsSystem(),
        new MatchSystem(),
        new SubWeaponSystem(),
        new ItemSpawnSystem(),
        new ItemSystem(),

        // physics resolution
        new Collision.HitboxHurtboxRetrievalSystem(),

        // animation-related systems
        new CustomAnimatorSystem(),
        new CustomAnimatorStateSystem(),
        new CustomAnimatorResetTriggersSystem()
      ];
    }
  }
}
