namespace Quantum.Movement
{
    public unsafe sealed class FastFallState : MovementState
    {
        public override States GetState() => States.IsFastFalling;

        public override bool GetInput(ref Input input) => input.FastFall;
        public override StateType GetStateType() => StateType.Aerial;

        protected override bool CanEnter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocity;
        }

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CharacterController->IsGrounded;
        }

        protected override void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.PhysicsBody->Velocity.Y = settings.FastFallForce;
        }
    }
}
