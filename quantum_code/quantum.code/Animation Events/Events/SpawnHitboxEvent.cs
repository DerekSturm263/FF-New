using Quantum.Collections;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnHitboxEvent : FrameEvent
    {
        public HitboxSettings Settings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            HitboxSystem.SpawnHitbox(f, Settings, EndingFrame - StartingFrame, entity);
        }
    }
}
