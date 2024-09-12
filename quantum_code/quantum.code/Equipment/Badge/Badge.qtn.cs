namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class Badge : InfoAsset
    {
        public virtual void OnApply(Frame f, ref CharacterControllerSystem.Filter filter) { }
        public virtual void OnUpdate(Frame f, ref CharacterControllerSystem.Filter filter) { }
        public virtual void OnRemove(Frame f, ref CharacterControllerSystem.Filter filter) { }
    }
}
