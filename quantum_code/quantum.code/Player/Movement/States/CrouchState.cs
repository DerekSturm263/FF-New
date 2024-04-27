namespace Quantum.Movement
{
    public unsafe sealed class CrouchState : MovementState
    {
        public override States GetState() => States.IsCrouching;

        public override bool GetInput(ref Input input) => input.Crouch;
        public override StateType GetStateType() => StateType.Grounded;

        protected override bool CanEnter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return input.Movement.X == 0;
        }

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return !input.Crouch;
        }
    }
}
