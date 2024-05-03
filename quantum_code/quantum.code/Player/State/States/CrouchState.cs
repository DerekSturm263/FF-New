namespace Quantum.Movement
{
    public unsafe sealed class CrouchState : PlayerState
    {
        public override States GetState() => States.IsCrouching;

        public override bool GetInput(ref Input input) => input.Crouch;
        public override StateType GetStateType() => StateType.Grounded;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => -1;

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return input.Movement.X == 0;
        }

        protected override bool CanExit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return !input.Crouch;
        }
    }
}
