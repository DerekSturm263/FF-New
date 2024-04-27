using System.Collections.Generic;

namespace Quantum
{
    public unsafe partial class QuantumAnimationEvent
    {
        public List<AssetRefFrameEvent> Events;

        public void InvokeEvents(Frame f, EntityRef entity, int frame)
        {
            for (int i = 0; i < Events.Count; ++i)
            {
                FrameEvent frameEvent = f.FindAsset<FrameEvent>(Events[i].Id);

                if (frame == frameEvent.StartingFrame)
                {
                    frameEvent.Begin(f, entity, frame);
                }
                else if (frame > frameEvent.StartingFrame && frame < frameEvent.EndingFrame)
                {
                    frameEvent.Update(f, entity, frame);
                }
                else if (frame == frameEvent.EndingFrame)
                {
                    frameEvent.End(f, entity, frame);
                }
            }
        }
    }
}
