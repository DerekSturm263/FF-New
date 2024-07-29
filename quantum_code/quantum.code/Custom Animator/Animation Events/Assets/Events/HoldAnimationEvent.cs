namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class HoldAnimationEvent : FrameEvent
    {
        public uint MaxLength;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Holding animation!");
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up animation holding!");
        }
    }
}
