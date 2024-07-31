using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class SubState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsUsingSubWeapon, 0);

        public override bool GetInput(ref Input input) => input.SubWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => 10;
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.SubUseTime;
        public override bool CanInterruptSelf => true;

        public override States[] EntranceBlacklist => new States[] { States.IsInteracting, States.IsBlocking, States.IsDodging, States.IsBursting };

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.CharacterController->IsCommitted)
                return false;

            if (filter.PlayerStats->Build.Equipment.Weapons.SubWeapon.Equals(default(Sub)))
                return false;

            if (f.TryFindAsset(filter.PlayerStats->Build.Equipment.Weapons.SubWeapon.Template.Id, out SubTemplate subWeapon))
                return filter.Stats->CurrentStats.Energy >= subWeapon.EnergyAmount;

            return false;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            AssetRefSubTemplate itemAsset = filter.PlayerStats->Build.Equipment.Weapons.SubWeapon.Template;
            if (f.TryFindAsset(itemAsset.Id, out SubTemplate subTemplate))
            {
                StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -subTemplate.EnergyAmount);

                ItemSpawnSettings itemSpawnSettings = new()
                {
                    Item = subTemplate.Prototype,
                    Velocity = FPVector2.Zero,
                    Offset = FPVector2.Zero,
                    StartHolding = false
                };

                ItemSpawnSystem.SpawnWithOwner(f, itemSpawnSettings, filter.Entity);
            }
        }

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalHelper.GetEnumFromDirection(input.Movement);
            if (filter.CharacterController->Direction == Direction.Neutral)
            {
                filter.CharacterController->Direction = DirectionalHelper.GetEnumFromDirection(new(filter.CharacterController->MovementDirection, 0));
            }

            if (!input.SubWeapon)
            {
                ItemSystem.Throw(f, filter.Entity, filter.PlayerStats->HeldItem, DirectionalHelper.GetFromDirection(settings.ThrowOffset, filter.CharacterController->Direction), DirectionalHelper.GetFromDirection(settings.ThrowForce, filter.CharacterController->Direction));
            }
        }
    }
}
