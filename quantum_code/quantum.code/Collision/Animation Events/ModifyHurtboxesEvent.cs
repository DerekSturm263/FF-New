namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ModifyHurtboxesEvent : FrameEvent
    {
        public HurtboxType Hurtboxes;
        public HurtboxSettings Settings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame)
        {
            Log.Debug("Modifying hurtboxes!");

            StatsSystem.ModifyHurtboxes(f, entity, Hurtboxes, Settings, false);
        }

        public override void End(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up hurtboxes!");

            StatsSystem.ModifyHurtboxes(f, entity, Hurtboxes, new() { CanBeDamaged = true, CanBeInterrupted = true, CanBeKnockedBack = true, DamageToBreak = 0 }, false);
        }
    }
}
