namespace Quantum
{
    public abstract unsafe partial class Badge
    {
        public virtual void OnApply(Frame f, EntityRef user) { }
        public virtual void OnUpdate(Frame f, EntityRef user) { }
        public virtual void OnRemove(Frame f, EntityRef user) { }
    }
}
