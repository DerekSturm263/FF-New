using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class DeadState : PassiveState
    {
        protected override bool IsInputting(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, ref Input input) => filter.Stats->IsDead;

        public override (States, StatesFlag) GetStateInfo() => (States.Dead, StatesFlag.Dead);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Default, transitionTime: 0, overrideExit: false, overrideEnter: false)
        ];

        private int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => settings.DeathTime;

        protected override bool DoExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->StateTime >= StateTime(f, stateMachine, ref filter, input, settings);

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            filter.Stats->IsRespawning = true;

            StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)32767, new() { CanBeDamaged = false, CanBeInterrupted = false, CanBeKnockedBack = false, DamageToBreak = int.MaxValue }, true);
            PlayerSpawnSystem.SetPosition(f, filter.Entity, FP._2 + FP._0_50);

            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
