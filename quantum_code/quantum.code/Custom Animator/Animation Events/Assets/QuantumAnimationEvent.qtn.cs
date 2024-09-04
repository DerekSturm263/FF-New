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

        public void InvokeEvents(Frame f, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            if (frame == Committed.Min)
            {
                Log.Debug("Committed!");

                filter.CharacterController->PossibleStates = 0;
                filter.CharacterController->CanMove = CanMove;
                filter.CharacterController->MaintainVelocity = MaintainVelocity;

                filter.CharacterController->ResetActions = true;
            }

            if (frame == Committed.Max)
            {
                filter.CharacterController->ResetActions = false;

                filter.CharacterController->MaintainVelocity = false;
                filter.CharacterController->CanMove = true;
                filter.CharacterController->PossibleStates = (StatesFlag)((int)StatesFlag.KnockedOver * 2 - 1);

                Log.Debug("Not committed!");
            }

            var list = f.ResolveList(filter.CustomAnimator->UnresolvedEvents);

            for (int i = 0; i < Events.Count; ++i)
            {
                FrameEvent frameEvent = f.FindAsset<FrameEvent>(Events[i].Id);

                if (frame == frameEvent.StartingFrame)
                {
                    Log.Debug("Event Begin");
                    frameEvent.Begin(f, this, ref filter, input, frame);

                    if (frameEvent.Length > 1)
                        list.Add(frameEvent);
                }
                else if (frame > frameEvent.StartingFrame && frame < frameEvent.EndingFrame)
                {
                    Log.Debug("Event Update");
                    frameEvent.Update(f, this, ref filter, input, frame, frame - frameEvent.StartingFrame);
                }
                else if (frame == frameEvent.EndingFrame)
                {
                    Log.Debug("Event End");
                    frameEvent.End(f, this, ref filter, input, frame);

                    if (frameEvent.Length > 1)
                        list.Remove(frameEvent);
                }
            }
        }
    }
}
