using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class KnockedOverState : PlayerState
    {
        [Header("State-Specific Values")]

        public FP MinimumYVelocityToKnockOver;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) && filter.CharacterController->CurrentKnockback.Direction.Magnitude >= MinimumYVelocityToKnockOver;
        }
    }
}
