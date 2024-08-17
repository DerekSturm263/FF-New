namespace Quantum
{
    public unsafe sealed class CrouchState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Crouch;

        public override (States, StatesFlag) GetStateInfo() => (States.Crouch, StatesFlag.Crouch);
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
            new() { Destination = States.LookUp },
            new(false, (f, filter, input, settings) => input.Movement.Magnitude < settings.DeadStickZone) { Destination = States.Default }
        ];

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            filter.CharacterController->Velocity = 0;
        }
    }
}
