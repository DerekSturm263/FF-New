using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class KnockedOverState : PlayerState
    {
        public AssetRefPlayerState Default;

        public FP MinimumYVelocityToKnockOver;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) && filter.PhysicsBody->Velocity.Y < MinimumYVelocityToKnockOver;
        }

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => true;
    }
}
