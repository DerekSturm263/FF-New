namespace Quantum
{
    public abstract unsafe partial class FrameEvent
    {
        public int StartingFrame;
        public int EndingFrame;

        public int Length => EndingFrame - StartingFrame;

        public abstract void Begin(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame);

        public virtual void Update(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame, int elapsedFrames) { }

        public virtual void End(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame) { }
    }
}
