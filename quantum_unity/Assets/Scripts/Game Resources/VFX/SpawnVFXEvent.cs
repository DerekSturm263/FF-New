using GameResources;
using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnVFXEvent : FrameEvent
    {
        public VFX Settings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning VFX!");

            VFXController.Instance.SpawnEffectParented(Settings, Object.FindFirstObjectByType<EntityViewUpdater>().GetView(entity).transform);
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up VFX!");
        }
    }
}
