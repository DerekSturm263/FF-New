using GameResources;
using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnVFXEvent : FrameEvent
    {
        public VFXSettings UnchargedSettings;
        public VFXSettings FullyChargedSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning VFX!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                VFXSettings settings = characterController->LerpFromAnimationHold_UNSAFE(VFXSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                VFXController.Instance.SpawnEffectParented(settings, Object.FindFirstObjectByType<EntityViewUpdater>().GetView(entity).transform);
            }
        }
    }
}
