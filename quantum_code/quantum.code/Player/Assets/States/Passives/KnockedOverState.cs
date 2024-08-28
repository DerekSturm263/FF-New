namespace Quantum
{
    public unsafe sealed class KnockedOverState : PassiveState
    {
        protected override bool IsInputting(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, ref Input input) => filter.CharacterController->GetNearbyCollider(Colliders.Ground);

        public override (States, StatesFlag) GetStateInfo() => (States.KnockedOver, StatesFlag.KnockedOver);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dead, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Knockback, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false)
        ];

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.LookUp))
            {
                stateMachine.BeginTransition(f, ref filter, input, settings, new(destination: States.Default, 0, false, false));
            }
        }

        protected override bool DoExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => true;
    }
}
