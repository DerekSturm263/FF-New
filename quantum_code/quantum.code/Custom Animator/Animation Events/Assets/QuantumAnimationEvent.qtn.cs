using Quantum.Inspector;
using System.Collections.Generic;

namespace Quantum
{
    public unsafe partial class QuantumAnimationEvent
    {
        [HideInInspector] public int AnimID;
        public List<AssetRefFrameEvent> Events;

        public void InvokeEvents(Frame f, EntityRef entity, int frame)
        {
            for (int i = 0; i < Events.Count; ++i)
            {
                FrameEvent frameEvent = f.FindAsset<FrameEvent>(Events[i].Id);

                if (frame == frameEvent.StartingFrame)
                {
                    Log.Debug("Event Begin");
                    frameEvent.Begin(f, entity, frame);
                }
                else if (frame > frameEvent.StartingFrame && frame < frameEvent.EndingFrame)
                {
                    Log.Debug("Event Update");
                    frameEvent.Update(f, entity, frame);
                }
                else if (frame == frameEvent.EndingFrame)
                {
                    Log.Debug("Event End");
                    frameEvent.End(f, entity, frame);
                }
            }
        }
    }
}
