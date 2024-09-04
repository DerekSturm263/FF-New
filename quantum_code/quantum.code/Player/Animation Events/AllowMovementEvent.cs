namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class AllowMovementEvent : FrameEvent
    {
        public StatesFlag AllowedStates;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Allowing movement!");

            filter.CharacterController->PossibleStates = AllowedStates;
        }

        public override void End(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Cleaning up movement allowance!");

            filter.CharacterController->PossibleStates = 0;
        }
    }
}
