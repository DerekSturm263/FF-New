using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class PlayClipEvent : FrameEvent
    {
        public ClipSettings UnchargedSettings;
        public ClipSettings FullyChargedSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Playing clip!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                ClipSettings settings = characterController->LerpFromAnimationHold_UNSAFE(ClipSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                AudioSource.PlayClipAtPoint(settings.GetClip(f.Unsafe.GetPointer<PlayerStats>(entity)->Build.Cosmetics.Voice), Vector3.zero, settings.Volume);
            }
        }
    }
}
