using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class SubWeaponState : PlayerState
    {
        public override States GetState() => States.IsUsingSubWeapon;

        public override bool GetInput(ref Input input) => input.SubWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => 10;
        protected override int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (f.TryFindAsset(filter.Stats->Build.Equipment.Weapons.SubWeapon.Template.Id, out SubWeaponTemplate subWeapon))
                return filter.Stats->CurrentEnergy >= subWeapon.EnergyAmount;

            return false;
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);
        }

        protected override void DelayedEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

            SubWeapon subWeaponAsset = filter.Stats->Build.Equipment.Weapons.SubWeapon;
            if (f.TryFindAsset(subWeaponAsset.Template.Id, out SubWeaponTemplate subWeapon))
            {
                StatsSystem.ModifyEnergy(f, filter.PlayerLink, filter.Stats, -subWeapon.EnergyAmount);
                EntityRef entity = SubWeaponSystem.Spawn(f, filter.Stats->Build.Equipment.Weapons.SubWeapon, filter.PlayerLink);

                if (input.SubWeapon)
                {
                    ItemSystem.PickUp(f, filter.Entity, entity);
                }
                else
                {
                    if (filter.CharacterController->Direction == Direction.Up)
                    ItemSystem.Throw(f, filter.Entity, entity, DirectionalAssetHelper.GetFromDirection(settings.ThrowForce, filter.CharacterController->Direction));
                }
            }
        }
    }
}
