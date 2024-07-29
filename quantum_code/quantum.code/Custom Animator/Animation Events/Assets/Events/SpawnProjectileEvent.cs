using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnProjectileEvent : FrameEvent
    {
        public ProjectileSettings Settings;
        public ProjectileSettings MaxHoldSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning projectile!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                ProjectileSettings settings;

                if (stats->MaxHoldAnimationFrameTime > 0)
                    settings = ProjectileSettings.Lerp(Settings, MaxHoldSettings, (FP)stats->HeldAnimationFrameTime / stats->MaxHoldAnimationFrameTime);
                else
                    settings = Settings;

                ProjectileHelper.SpawnProjectile(f, settings, entity);
            }
        }
    }
}
