using Photon.Deterministic;
using Quantum.Collections;
using System.Runtime;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ExplodingItem : HoldableItem
    {
        public HitboxSettings HitboxSettings;
        public int Lifetime;

        public override unsafe void OnHit(Frame f, PlayerLink* user, EntityRef target, EntityRef item, ItemInstance* itemInstance)
        {
            base.OnHit(f, user, target, item, itemInstance);

            HitboxSystem.SpawnHitbox(f, HitboxSettings, Lifetime, user->Entity);
        }
    }
}
