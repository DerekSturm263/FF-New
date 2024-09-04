using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class HoldAnimationEvent : FrameEvent
    {
        public Input.Buttons Button;

        public uint MinLength;
        public uint MaxLength;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Holding animation!");

            filter.CharacterController->HoldButton = (int)Button;

            filter.CharacterController->HeldAnimationFrameTime = 0;
            filter.CharacterController->MaxHoldAnimationFrameTime = (int)MaxLength;
            filter.CharacterController->CanMove = false;

            f.Events.OnPlayerHoldAnimation(filter.Entity, true);
        }

        public override void Update(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame, int elapsedFrames)
        {
            filter.CustomAnimator->speed = 0;
        }

        public override void End(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Cleaning up animation holding!");

            filter.CustomAnimator->speed = 1;

            f.Events.OnPlayerHoldAnimation(filter.Entity, false);

            filter.CharacterController->HoldButton = 0;

            if (filter.CharacterController->MaxHoldAnimationFrameTime != 0)
                filter.CharacterController->HoldLevel = (FP)filter.CharacterController->HeldAnimationFrameTime / filter.CharacterController->MaxHoldAnimationFrameTime;
            else
                filter.CharacterController->HoldLevel = 0;

            filter.CharacterController->HeldAnimationFrameTime = 0;
            filter.CharacterController->MaxHoldAnimationFrameTime = 0;
            filter.CharacterController->CanMove = parent.CanMove;
        }
    }
}
