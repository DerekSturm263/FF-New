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

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                characterController->HoldButton = (int)Button;

                characterController->HeldAnimationFrameTime = 0;
                characterController->MaxHoldAnimationFrameTime = (int)MaxLength;
            }
        }

        public override void Update(Frame f, EntityRef entity, int frame, int elapsedFrames)
        {
            base.Update(f, entity, frame, elapsedFrames);

            if (f.Unsafe.TryGetPointer(entity, out CustomAnimator* customAnimator))
                customAnimator->speed = 0;
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up animation holding!");

            if (f.Unsafe.TryGetPointer(entity, out CustomAnimator* customAnimator))
                customAnimator->speed = 1;

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                characterController->HoldButton = 0;

                characterController->HeldAnimationFrameTime = 0;
                characterController->MaxHoldAnimationFrameTime = 0;
            }
        }
    }
}
