using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class StatusEffect : InfoAsset
    {
        public int ActiveTime;
        public int TickRate;

        public abstract void OnApply(Frame f, EntityRef user);
        public virtual void OnTick(Frame f, EntityRef user) { }
        public abstract void OnRemove(Frame f, EntityRef user);
    }
}
