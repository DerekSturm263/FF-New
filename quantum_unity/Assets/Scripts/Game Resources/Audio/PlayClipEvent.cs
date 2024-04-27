using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class PlayClipEvent : FrameEvent
    {
        public Clip Settings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Playing clip!");

            AudioSource.PlayClipAtPoint(Settings.Variants.Random, Vector3.zero, Settings.Volume);
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up clip!");
        }
    }
}
