namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class DefaultState : PlayerState
    {
        public AssetRefPlayerState Interact;

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            if (filter.CharacterController->WasReleasedThisFrame(input, Input.Buttons.SubWeapon) && filter.CharacterController->HasSubWeapon)
            {
                stateMachine.ForceTransition(f, ref filter, input, settings, Interact, settings.InputCheckTime);
            }
        }

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => true;
    }
}
