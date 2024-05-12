namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class Badge : InfoAsset
    {
        public virtual void OnApply(Frame f, EntityRef user) { }
        public virtual void OnUpdate(Frame f, EntityRef user) { }
        public virtual void OnRemove(Frame f, EntityRef user) { }
    }
}
