namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ModifyHurtboxesEvent : FrameEvent
    {
        public HurtboxType Hurtboxes;
        public HurtboxSettings Settings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Modifying hurtboxes!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                var hurtboxes = f.ResolveDictionary(stats->Hurtboxes);

                for (int i = 0; i < 15; ++i)
                {
                    HurtboxType hurtboxType = (HurtboxType)(1 << i);
                    if (!Hurtboxes.HasFlag(hurtboxType))
                        continue;

                    if (f.Unsafe.TryGetPointer(hurtboxes[hurtboxType], out HurtboxInstance* hurtbox))
                    {
                        hurtbox->Settings = Settings;
                    }
                }

                f.Events.OnHurtboxStateChange(entity, Hurtboxes, Settings);
            }
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up hurtboxes!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                var hurtboxes = f.ResolveDictionary(stats->Hurtboxes);

                for (int i = 0; i < 15; ++i)
                {
                    HurtboxType hurtboxType = (HurtboxType)(1 << i);
                    if (!Hurtboxes.HasFlag(hurtboxType))
                        continue;

                    if (f.Unsafe.TryGetPointer(hurtboxes[hurtboxType], out HurtboxInstance* hurtbox))
                    {
                        hurtbox->Settings = new HurtboxSettings()
                        {
                            CanBeDamaged = true,
                            CanBeKnockedBack = true,
                            CanBeInterrupted = true,
                            DisableHitbox = true,
                        };
                    }
                }

                f.Events.OnHurtboxStateChange(entity, Hurtboxes, new HurtboxSettings()
                {
                    CanBeDamaged = true,
                    CanBeKnockedBack = true,
                    CanBeInterrupted = true,
                    DisableHitbox = true,
                });
            }
        }
    }
}
