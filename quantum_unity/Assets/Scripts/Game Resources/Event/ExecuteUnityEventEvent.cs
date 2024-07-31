using UnityEngine.Events;

namespace Quantum
{
    [System.Serializable]
    public struct UnityEventSettings
    {
        public UnityEvent OnBegin;
        public UnityEvent<int> OnUpdate;
        public UnityEvent OnEnd;

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

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Executing event!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                UnityEventSettings settings = characterController->LerpFromAnimationHold_UNSAFE(UnityEventSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                settings.OnBegin.Invoke();
            }
        }

        public override void Update(Frame f, EntityRef entity, int frame, int elapsedFrames)
        {
            Log.Debug("Updating event!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                UnityEventSettings settings = characterController->LerpFromAnimationHold_UNSAFE(UnityEventSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                settings.OnUpdate.Invoke(elapsedFrames);
            }
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up event!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                UnityEventSettings settings = characterController->LerpFromAnimationHold_UNSAFE(UnityEventSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                settings.OnEnd.Invoke();
            }
        }
    }
}
