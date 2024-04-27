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
                        hurtbox->Settings.CanBeDamaged = Settings.CanBeDamaged;
                        hurtbox->Settings.CanBeKnockedBack = Settings.CanBeKnockedBack;
                        hurtbox->Settings.CanBeInterrupted = Settings.CanBeInterrupted;
                        hurtbox->Settings.DisableHitbox = Settings.DisableHitbox;
                    }
                }
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
                        hurtbox->Settings.CanBeDamaged = true;
                        hurtbox->Settings.CanBeKnockedBack = true;
                        hurtbox->Settings.CanBeInterrupted = true;
                        hurtbox->Settings.DisableHitbox = false;
                    }
                }
            }
        }
    }
}
