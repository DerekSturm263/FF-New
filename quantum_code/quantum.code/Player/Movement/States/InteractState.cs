namespace Quantum.Movement
{
    public unsafe sealed class InteractState : MovementState
    {
        public override States GetState() => States.IsInteracting;

        public override bool GetInput(ref Input input) => input.Interact;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return true;
        }

        protected override void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);
        }
    }
}
