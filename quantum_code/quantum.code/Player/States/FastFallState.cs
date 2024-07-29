namespace Quantum
{
    public unsafe sealed class FastFallState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsFastFalling, StatesFlag.FastFall);

        public override bool GetInput(ref Input input) => input.FastFall;
        public override StateType GetStateType() => StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => -1;

        public override States[] KillStateList => new States[] { States.IsJumping };

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocity;
        }

        protected override bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.CharacterController->GetNearbyCollider(Colliders.Ground);
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.PhysicsBody->Velocity.Y = settings.FastFallForce;
        }
    }
}
