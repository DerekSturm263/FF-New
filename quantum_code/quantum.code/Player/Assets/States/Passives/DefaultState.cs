namespace Quantum
{
    public unsafe sealed class DefaultState : PassiveState
    {
        protected override bool IsInputting(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, ref Input input) => true;

        public override (States, StatesFlag) GetStateInfo() => (States.Default, StatesFlag.Default);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dead, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Knockback, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Burst, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Emote, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Block, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: false, overrideEnter: false),
        ];

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            HandleMovement(f, stateMachine, ref filter, input, settings, stats);
            HandleFastFalling(f, stateMachine, ref filter, input, settings, stats);

            if (filter.CharacterController->WasReleasedThisFrame(input, Input.Buttons.SubWeapon) && filter.CharacterController->HasSubWeapon)
            {
                stateMachine.BeginTransition(f, ref filter, input, settings, new(States.Interact, settings.InputCheckTime, true, true));
            }
        }

        private void HandleMovement(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            filter.CharacterController->Move(f, input.Movement.X, ref filter, settings, stats, 1);
        }

        private void HandleFastFalling(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Crouch) && filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocity)
            {
                filter.PhysicsBody->Velocity.Y = settings.FastFallForce;
            }
        }

        protected override bool DoExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => true;
    }
}
