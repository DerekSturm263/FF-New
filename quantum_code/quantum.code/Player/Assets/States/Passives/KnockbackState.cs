using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class KnockbackState : PassiveState
    {
        protected override bool IsInputting(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, ref Input input) => !filter.CharacterController->DeferredKnockback.Equals(default(FPVector2));

        public override (States, StatesFlag) GetStateInfo() => (States.Knockback, StatesFlag.Knockback);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dead, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Burst, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.KnockedOver, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: false, overrideEnter: false)
        ];

        protected override bool DoExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocityForKnockbackExit;
        }
    }
}
