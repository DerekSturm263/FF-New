namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class HitState : PlayerState
    {
        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return DoesStateTypeMatch(stateMachine, ref filter) && filter.CharacterController->CurrentKnockback.Equals(default(KnockbackInfo)) && filter.CharacterController->HitStunTime > 0;
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            --filter.CharacterController->HitStunTime;
        }

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.CharacterController->HitStunTime <= 0;
        }

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.CharacterController->HitStunTime = 0;

            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
