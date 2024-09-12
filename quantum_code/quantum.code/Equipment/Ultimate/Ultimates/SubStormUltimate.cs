namespace Quantum
{
    [System.Serializable]
    public unsafe partial class SubStormUltimate : Ultimate
    {
        public override void OnBegin(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            base.OnBegin(f, ref filter);
            
            filter.Stats->StatsMultiplier.Energy = 0;
        }

        public override void OnEnd(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.Stats->StatsMultiplier.Energy = 1;
        }
    }
}
