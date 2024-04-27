namespace Quantum.Movement
{
    public unsafe sealed class BlockState : MovementState
    {
        public override States GetState() => States.IsBlocking;

        public override bool GetInput(ref Input input) => input.Block;
        public override StateType GetStateType() => StateType.Grounded;

        public override States[] EntranceBlacklist => new States[] { States.IsBursting, States.IsDodging };

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return !input.Block || filter.CharacterController->IsInState(States.IsDodging);
        }
    }
}
