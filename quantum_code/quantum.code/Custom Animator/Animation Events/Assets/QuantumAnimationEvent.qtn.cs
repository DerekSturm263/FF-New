using Quantum.Inspector;
using System.Collections.Generic;

namespace Quantum
{
    public unsafe partial class QuantumAnimationEvent
    {
        [HideInInspector] public int AnimID;

        public List<AssetRefFrameEvent> Events;
        public Range Committed;

        public void InvokeEvents(Frame f, EntityRef entity, int frame)
        {
            if (frame == Committed.Min)
            {
                Log.Debug("Committed!");

                if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
                    characterController->IsCommitted = true;
            }
            if (frame == Committed.Max)
            {
                if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
                    characterController->IsCommitted = false;

                Log.Debug("Not committed!");
            }

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
                    frameEvent.Update(f, entity, frame, frame - frameEvent.StartingFrame);
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
