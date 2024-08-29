namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class HoldAnimationEvent : FrameEvent
    {
        public Input.Buttons Button;

        public uint MinLength;
        public uint MaxLength;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Holding animation!");

            if (f.Unsafe.TryGetPointer(entity, out CustomAnimator* customAnimator))
                customAnimator->speed = 0;

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                characterController->HoldButton = (int)Button;
            }
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up animation holding!");

            if (f.Unsafe.TryGetPointer(entity, out CustomAnimator* customAnimator))
                customAnimator->speed = 1;

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                characterController->HoldButton = -1;
            }
        }
    }
}
