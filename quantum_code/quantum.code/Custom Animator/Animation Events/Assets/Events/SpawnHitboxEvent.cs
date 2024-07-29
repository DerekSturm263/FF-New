using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnHitboxEvent : FrameEvent
    {
        public HitboxSettings Settings;
        public HitboxSettings MaxHoldSettings;

        public Shape2DConfig Shape;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning hitbox!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                HitboxSettings settings;

                if (stats->MaxHoldAnimationFrameTime > 0)
                    settings = HitboxSettings.Lerp(Settings, MaxHoldSettings, (FP)stats->HeldAnimationFrameTime / stats->MaxHoldAnimationFrameTime);
                else
                    settings = Settings;
                
                HitboxSystem.SpawnHitbox(f, settings, Shape, Length, entity);
            }
        }
    }
}
