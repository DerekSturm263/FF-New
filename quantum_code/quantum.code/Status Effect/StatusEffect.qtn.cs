using Photon.Deterministic;

namespace Quantum
{
    public abstract unsafe partial class StatusEffect
    {
        public int ActiveTime;
        public int TickRate;

        public abstract void OnApply(Frame f, EntityRef user);
        public virtual void OnTick(Frame f, EntityRef user) { }
        public abstract void OnRemove(Frame f, EntityRef user);
    }
}
