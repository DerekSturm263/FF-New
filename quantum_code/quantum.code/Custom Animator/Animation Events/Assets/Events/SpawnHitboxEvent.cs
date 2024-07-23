using Quantum.Collections;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnHitboxEvent : FrameEvent
    {
        public HitboxSettings Settings;
        public Shape2DConfig Shape;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            HitboxSystem.SpawnHitbox(f, Settings, Shape, EndingFrame - StartingFrame, entity);
        }
    }
}
