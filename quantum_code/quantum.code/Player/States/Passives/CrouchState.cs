namespace Quantum
{
    public unsafe sealed class CrouchState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Crouch;

        public override (States, StatesFlag) GetStateInfo() => (States.Crouch, StatesFlag.Crouch);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;
        
        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Emote, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: false, overrideEnter: false)
        ];

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            if (filter.CharacterController->WasReleasedThisFrame(input, Input.Buttons.SubWeapon) && filter.CharacterController->HasSubWeapon)
            {
                stateMachine.BeginTransition(f, ref filter, input, settings, new(States.Interact, settings.InputCheckTime, true, true));
            }
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.CharacterController->Velocity = 0;
        }
    }
}
