using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class SubState : PlayerState
    {
        public override States GetState() => States.IsUsingSubWeapon;

        public override bool GetInput(ref Input input) => input.SubWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => 10;
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.SubUseTime;
        public override bool CanInterruptSelf => true;

        public override States[] EntranceBlacklist => new States[] { States.IsInteracting, States.IsBlocking, States.IsDodging, States.IsBursting };

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.Stats->Build.Equipment.Weapons.SubWeapon.Equals(default(Sub)))
                return false;

            if (f.TryFindAsset(filter.Stats->Build.Equipment.Weapons.SubWeapon.Template.Id, out SubTemplate subWeapon))
                return filter.Stats->CurrentEnergy >= subWeapon.EnergyAmount;

            return false;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            AssetRefSubTemplate itemAsset = filter.Stats->Build.Equipment.Weapons.SubWeapon.Template;
            if (f.TryFindAsset(itemAsset.Id, out SubTemplate subTemplate))
            {
                StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -subTemplate.EnergyAmount);

                EntityRef instance = ItemSpawnSystem.SpawnInHand(f, subTemplate.Prototype, filter.Entity);
                if (f.Unsafe.TryGetPointer(instance, out SubInstance* subInstance))
                {
                    subInstance->SubWeapon = filter.Stats->Build.Equipment.Weapons.SubWeapon;
                }
            }
        }

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);
            if (filter.CharacterController->Direction == Direction.Neutral)
            {
                filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(new(filter.CharacterController->MovementDirection, 0));
            }

            if (!input.SubWeapon)
            {
                ItemSystem.Throw(f, filter.Entity, filter.Stats->HeldItem, DirectionalAssetHelper.GetFromDirection(settings.ThrowOffset, filter.CharacterController->Direction), DirectionalAssetHelper.GetFromDirection(settings.ThrowForce, filter.CharacterController->Direction));
            }
        }
    }
}
