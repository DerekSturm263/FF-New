namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class AllowMovementEvent : FrameEvent
    {
        public StatesFlag AllowedStates;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Allowing movement!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                characterController->PossibleStates = AllowedStates;
            }
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up movement allowance!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                characterController->PossibleStates = 0;
            }
        }
    }
}
