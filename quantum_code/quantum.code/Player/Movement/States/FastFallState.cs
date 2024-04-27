namespace Quantum.Movement
{
    public unsafe sealed class FastFallState : PlayerState
    {
        public override States GetState() => States.IsFastFalling;

        public override bool GetInput(ref Input input) => input.FastFall;
        public override StateType GetStateType() => StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => -1;

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocity;
        }

        protected override bool CanExit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CharacterController->GetNearbyCollider(Colliders.Ground);
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.PhysicsBody->Velocity.Y = settings.FastFallForce;
        }
    }
}
