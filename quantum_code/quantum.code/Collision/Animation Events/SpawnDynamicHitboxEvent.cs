using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnDynamicHitboxEvent : FrameEvent
    {
        public Shape2DConfig Shape;

        public HitboxSettings Settings;
        public HitboxSettings MaxHoldSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning dynamic hitbox!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                HitboxSettings settings = characterController->LerpFromAnimationHold(HitboxSettings.Lerp, Settings, MaxHoldSettings);
                HitboxSystem.SpawnDynamicHitbox(f, settings, Shape, Length, entity);
            }
        }
    }
}
