namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ModifyHurtboxesEvent : FrameEvent
    {
        public HurtboxType Hurtboxes;
        public HurtboxSettings Settings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Modifying hurtboxes!");

            StatsSystem.ModifyHurtboxes(f, filter.Entity, Hurtboxes, Settings, false);
        }

        public override void End(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Cleaning up hurtboxes!");

            StatsSystem.ResetHurtboxes(f, filter.Entity, Hurtboxes, false);
        }
    }
}
