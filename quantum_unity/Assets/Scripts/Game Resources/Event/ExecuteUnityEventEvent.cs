using UnityEngine.Events;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ExecuteUnityEvent : FrameEvent
    {
        public UnityEvent OnBegin;
        public UnityEvent<int> OnUpdate;
        public UnityEvent OnEnd;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Executing event!");

            OnBegin.Invoke();
        }

        public override void Update(Frame f, EntityRef entity, int frame, int elapsedFrames)
        {
            Log.Debug("Updating event!");

            OnUpdate.Invoke(elapsedFrames);
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up event!");

            OnEnd.Invoke();
        }
    }
}
