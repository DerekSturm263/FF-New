﻿using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class Item : InfoAsset
    {
        public AssetRefEntityPrototype Prototype;
        public bool DestroySelfOnHit;

        public abstract void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance);
        public virtual void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef item, ItemInstance* itemInstance)
        {
            if (f.Unsafe.TryGetPointer(item, out Transform2D* transform))
                f.Events.OnItemUse(f.Get<PlayerLink>(user), item, this, transform->Position);

            if (DestroySelfOnHit)
            {
                f.Destroy(item);

                if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                    --itemSpawner->CurrentSpawned;
            }
        }
    }
}
