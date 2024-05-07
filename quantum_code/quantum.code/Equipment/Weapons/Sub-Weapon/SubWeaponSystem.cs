using Photon.Deterministic;

namespace Quantum
{
    public unsafe class SubWeaponSystem : SystemMainThreadFilter<SubWeaponSystem.Filter>, ISignalOnCollisionEnter2D
    {
        public struct Filter
        {
            public EntityRef Entity;

            public SubWeaponInstance* SubWeaponInstance;
            public ItemInstance* ItemInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (f.TryFindAsset(filter.SubWeaponInstance->SubWeapon.Enhancers.Enhancer1.Id, out SubWeaponEnhancer enhancer))
                enhancer.OnUpdate(f, filter.ItemInstance->Owner, filter.SubWeaponInstance->Target, filter.Entity, filter.SubWeaponInstance);

            SubWeaponTemplate template = f.FindAsset<SubWeaponTemplate>(filter.SubWeaponInstance->SubWeapon.Template.Id);
            template.OnUpdate(f, filter.ItemInstance->Owner, filter.SubWeaponInstance->Target, filter.Entity, filter.SubWeaponInstance);
        }

        public static EntityRef Spawn(Frame f, SubWeapon subWeapon, PlayerLink* player)
        {
            SubWeaponTemplate template = f.FindAsset<SubWeaponTemplate>(subWeapon.Template.Id);
            EntityRef entity = f.Create(f.FindAsset<EntityPrototype>(template.Prototype.Id));

            if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform) &&
                f.Unsafe.TryGetPointer(player->Entity, out Stats* stats))
            {
                var dict = f.ResolveDictionary(stats->Hurtboxes);

                if (f.Unsafe.TryGetPointer(dict[HurtboxType.RightHand], out Transform2D* parentTransform))
                    transform->Position = parentTransform->Position;

                if (f.Unsafe.TryGetPointer(entity, out ItemInstance* itemInstance))
                    itemInstance->Owner = player->Entity;

                if (f.TryFindAsset(subWeapon.Enhancers.Enhancer1.Id, out SubWeaponEnhancer enhancer))
                    enhancer.OnSpawn(f, player->Entity, entity, f.Unsafe.GetPointer<SubWeaponInstance>(entity));

                template.OnSpawn(f, player->Entity, entity, f.Unsafe.GetPointer<SubWeaponInstance>(entity));
            }

            if (f.Unsafe.TryGetPointer(entity, out SubWeaponInstance* subWeaponInstance))
                subWeaponInstance->SubWeapon = subWeapon;

            return entity;
        }

        public void OnCollisionEnter2D(Frame f, CollisionInfo2D info)
        {
            if (f.Unsafe.TryGetPointer(info.Entity, out SubWeaponInstance* subWeaponInstance) &&
                f.Unsafe.TryGetPointer(info.Entity, out ItemInstance* itemInstance))
            {
                if (itemInstance->Holder.IsValid)
                    return;

                if (info.Other == itemInstance->Owner)
                    return;

                if (f.TryFindAsset(subWeaponInstance->SubWeapon.Template.Id, out SubWeaponTemplate subWeaponTemplate))
                {
                    if (f.TryFindAsset(subWeaponInstance->SubWeapon.Enhancers.Enhancer1.Id, out SubWeaponEnhancer enhancer))
                        enhancer.OnHit(f, itemInstance->Owner, info.Other, info.Entity, subWeaponInstance);

                    subWeaponTemplate.OnHit(f, itemInstance->Owner, info.Other, info.Entity, subWeaponInstance);
                }
            }
        }
    }
}
