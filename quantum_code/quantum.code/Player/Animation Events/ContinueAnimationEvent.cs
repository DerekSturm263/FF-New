namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ContinueAnimationEvent : FrameEvent
    {
        public AssetRefPlayerState Default;
        public Input.Buttons Button;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Continuing animation!");

            filter.CharacterController->PressedButton = false;
        }

        public override void Update(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame, int elapsedFrames)
        {
            if (filter.CharacterController->WasPressedThisFrame(input, Button))
                filter.CharacterController->PressedButton = true;
        }

        public override void End(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Cleaning up animation continuing!");

            if (!filter.CharacterController->PressedButton)
            {
                MovementSettings settings = f.FindAsset<MovementSettings>(filter.CharacterController->Settings.Id);

                CharacterControllerSystem.StateMachine.ForceTransition(f, ref filter, input, settings, Default, 0);
                filter.CharacterController->PossibleStates = (StatesFlag)((int)StatesFlag.KnockedOver * 2 - 1);
            }

            filter.CharacterController->PressedButton = false;
        }
    }
}
