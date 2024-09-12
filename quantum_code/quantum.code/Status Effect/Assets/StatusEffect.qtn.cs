namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class StatusEffect : InfoAsset
    {
        public int ActiveTime;
        public int TickRate;

        public abstract void OnApply(Frame f, ref CharacterControllerSystem.Filter filter);
        public virtual void OnTick(Frame f, ref CharacterControllerSystem.Filter filter) { }
        public abstract void OnRemove(Frame f, ref CharacterControllerSystem.Filter filter);
    }
}
