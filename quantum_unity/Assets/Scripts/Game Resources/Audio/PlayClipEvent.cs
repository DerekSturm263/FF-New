using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class PlayClipEvent : FrameEvent
    {
        public ClipSettings UnchargedSettings;
        public ClipSettings FullyChargedSettings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Playing clip!");

            ClipSettings settings = filter.CharacterController->LerpFromAnimationHold_UNSAFE(ClipSettings.Lerp, UnchargedSettings, FullyChargedSettings);
            AudioSource.PlayClipAtPoint(settings.GetClip(filter.PlayerStats->Build.Frame.Voice), Vector3.zero, settings.Volume);
        }
    }
}
