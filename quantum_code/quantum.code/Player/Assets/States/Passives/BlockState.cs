using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class BlockState : PlayerState
    {
        public AssetRefPlayerState Dodge;

        public FPVector2 KnockbackMultiplier;

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.CharacterController->WasReleasedThisFrame(input, Input.Buttons.Block);
        }
    }
}
