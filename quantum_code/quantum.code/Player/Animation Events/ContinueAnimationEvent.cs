namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ContinueAnimationEvent : FrameEvent
    {
        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Continuing animation!");
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up animation continuing!");
        }
    }
}
