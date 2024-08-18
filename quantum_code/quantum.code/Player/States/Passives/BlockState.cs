namespace Quantum
{
    public unsafe sealed class BlockState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Block;

        public override (States, StatesFlag) GetStateInfo() => (States.Block, StatesFlag.Block);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;

        public override TransitionInfo[] GetTransitions(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 4, overrideExit: true, overrideEnter: false),
            new(destination: States.Burst, transitionTime: 4, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 6, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 6, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 6, overrideExit: false, overrideEnter: false)
        ];

        public override void FinishEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, ref filter, input, settings, previousState);

            filter.CharacterController->Velocity = 0;
        }
    }
}
