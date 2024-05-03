namespace Quantum.Movement
{
    public unsafe sealed class BlockState : PlayerState
    {
        public override States GetState() => States.IsBlocking;

        public override bool GetInput(ref Input input) => input.Block;
        public override StateType GetStateType() => StateType.Grounded;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => -1;

        public override States[] EntranceBlacklist => new States[] { States.IsBursting, States.IsDodging };

        protected override bool CanExit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return !input.Block || filter.CharacterController->IsInState(States.IsDodging);
        }
    }
}
