using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ExplodingSubWeaponTemplate : SubWeaponTemplate
    {
        public HitboxSettings HitboxSettings;
        public int Lifetime;

        public unsafe void OnHit(Frame f, PlayerLink* user, EntityRef target)
        {
            HitboxSystem.SpawnHitbox(f, HitboxSettings, Lifetime, user->Entity);
        }
    }
}
