namespace Quantum
{
    public unsafe sealed class LookUpState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.LookUp;

        public override (States, StatesFlag) GetStateInfo() => (States.LookUp, StatesFlag.LookUp);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;

        public override TransitionInfo[] GetTransitions() =>
        [
            new(true) { Destination = States.Dodge },
            new() { Destination = States.Emote },
            new(true) { Destination = States.Interact },
            new(true) { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new(true) { Destination = States.Sub },
            new() { Destination = States.Crouch },
            new() { Destination = States.Default }
        ];

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            filter.CharacterController->Velocity = 0;
        }
    }
}
