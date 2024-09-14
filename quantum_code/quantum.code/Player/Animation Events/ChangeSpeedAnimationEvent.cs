using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ChangeSpeedAnimationEvent : FrameEvent
    {
        public FP Speed;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Holding animation!");

            filter.CustomAnimator->speed = Speed;
        }

        public override void End(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Cleaning up animation holding!");

            filter.CustomAnimator->speed = 1;
        }
    }
}
