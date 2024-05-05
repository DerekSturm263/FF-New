using Photon.Deterministic;

namespace Quantum
{
    public unsafe abstract partial class MainWeaponEnhancer
    {
        public virtual void OnMoveStart(Frame f, EntityRef user, ref HitboxSettings hitboxSettings) { }
        public virtual void OnMoveUpdate(Frame f, EntityRef user) { }
        public virtual void OnMoveEnd(Frame f, EntityRef user) { }
        public virtual void OnHit(Frame f, EntityRef user, EntityRef target) { }
    }
}
