using GameResources;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnVFXEvent : FrameEvent
    {
        public VFXSettings Settings;
        public VFXSettings MaxHoldSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning VFX!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                VFXSettings settings;

                if (stats->MaxHoldAnimationFrameTime > 0)
                    settings = VFXSettings.Lerp(Settings, MaxHoldSettings, ((FP)stats->HeldAnimationFrameTime / stats->MaxHoldAnimationFrameTime).AsFloat);
                else
                    settings = Settings;

                VFXController.Instance.SpawnEffectParented(Settings, Object.FindFirstObjectByType<EntityViewUpdater>().GetView(entity).transform);
            }
        }
    }
}
