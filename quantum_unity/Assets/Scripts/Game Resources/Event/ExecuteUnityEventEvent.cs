using UnityEngine.Events;

namespace Quantum
{
    [System.Serializable]
    public struct UnityEventSettings
    {
        public UnityEvent<CharacterControllerSystem.Filter> OnBegin;
        public UnityEvent<CharacterControllerSystem.Filter, int> OnUpdate;
        public UnityEvent<CharacterControllerSystem.Filter> OnEnd;

        public static UnityEventSettings Lerp(UnityEventSettings a, UnityEventSettings b, float c)
        {
            if (c == 1)
                return b;
            else
                return a;
        }
    }

    [System.Serializable]
    public sealed unsafe partial class ExecuteUnityEvent : FrameEvent
    {
        public UnityEventSettings UnchargedSettings;
        public UnityEventSettings FullyChargedSettings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Executing event!");

            UnityEventSettings settings = filter.CharacterController->LerpFromAnimationHold_UNSAFE(UnityEventSettings.Lerp, UnchargedSettings, FullyChargedSettings);
            settings.OnBegin.Invoke(filter);
        }

        public override void Update(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame, int elapsedFrames)
        {
            Log.Debug("Updating event!");

            UnityEventSettings settings = filter.CharacterController->LerpFromAnimationHold_UNSAFE(UnityEventSettings.Lerp, UnchargedSettings, FullyChargedSettings);
            settings.OnUpdate.Invoke(filter, elapsedFrames);
        }

        public override void End(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Cleaning up event!");

            UnityEventSettings settings = filter.CharacterController->LerpFromAnimationHold_UNSAFE(UnityEventSettings.Lerp, UnchargedSettings, FullyChargedSettings);
            settings.OnEnd.Invoke(filter);
        }
    }
}
