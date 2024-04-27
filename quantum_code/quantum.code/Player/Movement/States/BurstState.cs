namespace Quantum.Movement
{
    public unsafe sealed class BurstState : MovementState
    {
        public override States GetState() => States.IsBursting;

        public override bool GetInput(ref Input input) => input.Burst;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;

        public override States[] KillStateList => new States[] { States.IsBlocking, States.IsDodging };

        protected override bool CanEnter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            if (filter.Stats->CurrentEnergy < settings.BurstCost)
                return false;

            return true;
        }

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CharacterController->BurstTime >= settings.BurstFrames;
        }

        protected override void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.PhysicsBody->GravityScale = 0;
            StatsSystem.ModifyEnergy(f, filter.PlayerLink, filter.Stats, -settings.BurstCost);
        }

        public override void Update(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, ref input, settings);

            ++filter.CharacterController->BurstTime;
        }

        protected override void Exit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, ref input, settings);

            filter.PhysicsBody->GravityScale = 1;
            filter.CharacterController->BurstTime = 0;
        }
    }
}
