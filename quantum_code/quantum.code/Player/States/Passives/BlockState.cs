namespace Quantum
{
    public unsafe sealed class BlockState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Block;

        public override (States, StatesFlag) GetStateInfo() => (States.Block, StatesFlag.Block);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;

        public override TransitionInfo[] GetTransitions() =>
        [
            new(true) { Destination = States.Dodge },
            new(true) { Destination = States.Sub },
            new() { Destination = States.Burst },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            filter.CharacterController->Velocity = 0;
        }
    }
}
