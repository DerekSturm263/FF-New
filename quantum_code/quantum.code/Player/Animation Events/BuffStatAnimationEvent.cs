namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class BuffStatAnimationEvent : FrameEvent
    {
        public ApparelStats ApparelStatsMultiplier;
        public WeaponStats WeaponStatsMultiplier;

        public int DefaultTime;

        public override void Begin(Frame f, QuantumAnimationEvent parent, ref CharacterControllerSystem.Filter filter, Input input, int frame)
        {
            Log.Debug("Buffing a stat!");

            filter.PlayerStats->ApparelStatsMultiplier = ApparelStatsMultiplier;
            filter.PlayerStats->WeaponStatsMultiplier = WeaponStatsMultiplier;

            filter.CharacterController->DoResetBuffs = true;
        }
    }
}
