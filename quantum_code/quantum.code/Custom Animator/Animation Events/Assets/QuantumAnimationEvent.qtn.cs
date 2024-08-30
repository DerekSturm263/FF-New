using Quantum.Inspector;
using System.Collections.Generic;

namespace Quantum
{
    public unsafe partial class QuantumAnimationEvent
    {
        [HideInInspector] public int AnimID;

        public List<AssetRefFrameEvent> Events;
        public Range Committed;

        public bool CanMove;
        public bool MaintainVelocity;

        public void InvokeEvents(Frame f, EntityRef entity, int frame)
        {
            if (frame == Committed.Min)
            {
                Log.Debug("Committed!");

                if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
                {
                    characterController->PossibleStates = 0;
                    characterController->CanMove = CanMove;
                    characterController->MaintainVelocity = MaintainVelocity;
                
                }
            }
            if (frame == Committed.Max)
            {
                if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
                {
                    characterController->PossibleStates = (StatesFlag)((int)StatesFlag.KnockedOver * 2 - 1);
                    characterController->CanMove = true;
                    characterController->MaintainVelocity = false;
                }

                Log.Debug("Not committed!");
            }

            for (int i = 0; i < Events.Count; ++i)
            {
                FrameEvent frameEvent = f.FindAsset<FrameEvent>(Events[i].Id);

                if (frame == frameEvent.StartingFrame)
                {
                    Log.Debug("Event Begin");
                    frameEvent.Begin(f, this, entity, frame);
                }
                else if (frame > frameEvent.StartingFrame && frame < frameEvent.EndingFrame)
                {
                    Log.Debug("Event Update");
                    frameEvent.Update(f, this, entity, frame, frame - frameEvent.StartingFrame);
                }
                else if (frame == frameEvent.EndingFrame)
                {
                    Log.Debug("Event End");
                    frameEvent.End(f, this, entity, frame);
                }
            }
        }
    }
}
