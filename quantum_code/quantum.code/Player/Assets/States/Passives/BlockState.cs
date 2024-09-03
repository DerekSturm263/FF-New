using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class BlockState : PlayerState
    {
        public AssetRefPlayerState Dodge;

        public FPVector2 KnockbackMultiplier;

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);
        
            filter.CharacterController->KnockbackMultiplier = KnockbackMultiplier;
        }

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.CharacterController->WasReleasedThisFrame(input, Input.Buttons.Block);
        }

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.CharacterController->KnockbackMultiplier = FPVector2.One;

            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
