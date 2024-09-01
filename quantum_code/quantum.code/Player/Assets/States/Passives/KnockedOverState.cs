namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class KnockedOverState : PlayerState
    {
        public AssetRefPlayerState Default;

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => true;
    }
}
