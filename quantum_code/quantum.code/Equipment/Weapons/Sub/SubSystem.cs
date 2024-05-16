using Photon.Deterministic;

namespace Quantum
{
    // TODO: PHASE OUT. SUB-WEAPONS ARE JUST A FANCY WAY TO SPAWN SPECIAL ITEMS
    public unsafe class SubSystem : SystemMainThreadFilter<SubSystem.Filter>, ISignalOnCollisionEnter2D
    {
        public struct Filter
        {
            public EntityRef Entity;

            public SubInstance* SubInstance;
            public ItemInstance* ItemInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (f.TryFindAsset(filter.SubInstance->SubWeapon.Enhancers.Enhancer1.Id, out SubEnhancer enhancer))
                enhancer.OnUpdate(f, filter.ItemInstance->Owner, filter.SubInstance->Target, filter.Entity, filter.SubInstance);

            SubTemplate template = f.FindAsset<SubTemplate>(filter.SubInstance->SubWeapon.Template.Id);
            template.OnUpdate(f, filter.ItemInstance->Owner, filter.SubInstance->Target, filter.Entity, filter.SubInstance);
        }

        public static EntityRef Spawn(Frame f, Sub subWeapon, PlayerLink* player)
        {
            SubTemplate template = f.FindAsset<SubTemplate>(subWeapon.Template.Id);
            EntityRef entity = f.Create(f.FindAsset<EntityPrototype>(template.Prototype.Id));

            if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform) &&
                f.Unsafe.TryGetPointer(player->Entity, out Stats* stats))
            {
                var dict = f.ResolveDictionary(stats->Hurtboxes);

                if (f.Unsafe.TryGetPointer(dict[HurtboxType.RightHand], out Transform2D* parentTransform))
                    transform->Position = parentTransform->Position;

                if (f.Unsafe.TryGetPointer(entity, out ItemInstance* itemInstance))
                    itemInstance->Owner = player->Entity;

                if (f.TryFindAsset(subWeapon.Enhancers.Enhancer1.Id, out SubEnhancer enhancer))
                    enhancer.OnSpawn(f, player->Entity, entity, f.Unsafe.GetPointer<SubInstance>(entity));

                template.OnSpawn(f, player->Entity, entity, f.Unsafe.GetPointer<SubInstance>(entity));
            }

            if (f.Unsafe.TryGetPointer(entity, out SubInstance* subWeaponInstance))
                subWeaponInstance->SubWeapon = subWeapon;

            return entity;
        }

        public void OnCollisionEnter2D(Frame f, CollisionInfo2D info)
        {
            if (f.Unsafe.TryGetPointer(info.Entity, out SubInstance* subWeaponInstance) &&
                f.Unsafe.TryGetPointer(info.Entity, out ItemInstance* itemInstance))
            {
                if (itemInstance->Holder.IsValid)
                    return;

                if (info.Other == itemInstance->Owner)
                    return;

                if (f.TryFindAsset(subWeaponInstance->SubWeapon.Template.Id, out SubTemplate subWeaponTemplate))
                {
                    if (f.TryFindAsset(subWeaponInstance->SubWeapon.Enhancers.Enhancer1.Id, out SubEnhancer enhancer))
                        enhancer.OnHit(f, itemInstance->Owner, info.Other, info.Entity, subWeaponInstance);

                    subWeaponTemplate.OnHit(f, itemInstance->Owner, info.Other, info.Entity, subWeaponInstance);
                }
            }
        }
    }
}
