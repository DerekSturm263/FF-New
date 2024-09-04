using GameResources;
using UnityEngine;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnVFXEvent : FrameEvent
    {
        public VFXSettings UnchargedSettings;
        public VFXSettings FullyChargedSettings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Spawning VFX!");

            VFXSettings settings = filter.CharacterController->LerpFromAnimationHold_UNSAFE(VFXSettings.Lerp, UnchargedSettings, FullyChargedSettings);
            VFXController.Instance.SpawnEffectParented(settings, Object.FindFirstObjectByType<EntityViewUpdater>().GetView(filter.Entity).transform);
        }
    }
}
