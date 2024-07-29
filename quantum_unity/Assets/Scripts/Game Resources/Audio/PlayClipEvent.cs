using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class PlayClipEvent : FrameEvent
    {
        public ClipSettings Settings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Playing clip!");

            AudioSource.PlayClipAtPoint(Settings.GetClip(f.Unsafe.GetPointer<PlayerStats>(entity)->Build.Cosmetics.Voice), Vector3.zero, Settings.Volume);
        }
    }
}
