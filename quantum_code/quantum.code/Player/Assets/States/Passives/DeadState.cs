using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class DeadState : PlayerState
    {
        [Header("State-Specific Values")]

        public FP SpawnHeight;
        public HurtboxSettings RespawnSettings;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) && filter.Stats->IsDead;
        }

        public override void BeginEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.BeginEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.CharacterController->CurrentKnockback = default;
            StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)((int)HurtboxType.Head * 2 - 1), RespawnSettings, true);
        }

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->StateTime >= f.Global->CurrentMatch.Ruleset.Players.RespawnTime * 60 && (filter.Stats->CurrentStats.Stocks > 0 || f.Global->CurrentMatch.Ruleset.Players.StockCount == -1);

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.Stats->IsRespawning = true;

            filter.CharacterController->CurrentKnockback = default;
            filter.CharacterController->DeferredKnockback = default;
            filter.CharacterController->OldKnockback = default;

            StatsSystem.SetEnergy(f, filter.Entity, filter.Stats, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy / 5);
            PlayerSpawnSystem.SetPosition(f, filter.Entity, SpawnHeight);

            f.Events.OnPlayerRespawn(new() { Entity = filter.Entity, Index = filter.PlayerStats->Index, Position = filter.Transform->Position });

            filter.Stats->IsDead = false;

            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
