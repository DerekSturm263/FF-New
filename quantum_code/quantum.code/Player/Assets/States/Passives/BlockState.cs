namespace Quantum
{
    public unsafe sealed class BlockState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Block;

        public override (States, StatesFlag) GetStateInfo() => (States.Block, StatesFlag.Block);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dead, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Knockback, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 4, overrideExit: true, overrideEnter: false),
            new(destination: States.Burst, transitionTime: 2, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 8, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 8, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 12, overrideExit: false, overrideEnter: false)
        ];

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.CharacterController->Velocity = 0;
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.LeftRight) || filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Crouch))
            {
                stateMachine.BeginTransition(f, ref filter, input, settings, new(States.Dodge, settings.InputCheckTime, false, false));
            }
        }
    }
}
