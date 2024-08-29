using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class DeadState : PlayerState
    {
        public int Time;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) && filter.Stats->IsDead;
        }

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->StateTime >= Time;

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.Stats->IsRespawning = true;

            StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)32767, new() { CanBeDamaged = false, CanBeInterrupted = false, CanBeKnockedBack = false, DamageToBreak = int.MaxValue }, true);
            PlayerSpawnSystem.SetPosition(f, filter.Entity, FP._2 + FP._0_50);

            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
