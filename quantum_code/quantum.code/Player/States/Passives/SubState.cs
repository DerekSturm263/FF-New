using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class SubState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.SubWeapon;

        public override (States, StatesFlag) GetStateInfo() => (States.Sub, 0);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Burst },
            new() { Destination = States.Dodge },
            new() { Destination = States.Emote },
            new() { Destination = States.Interact },
            new() { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new() { Destination = States.Sub },
            new() { Destination = States.Ultimate },
            new() { Destination = States.Block },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (!base.CanEnter(f, ref filter, input, settings))
                return false;

            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.SubWeapon.Template.Id, out SubTemplate subWeapon))
                return subWeapon.Prototype.Id.IsValid && filter.Stats->CurrentStats.Energy >= subWeapon.EnergyAmount;

            return false;
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            AssetRefSubTemplate itemAsset = filter.PlayerStats->Build.Gear.SubWeapon.Template;
            if (f.TryFindAsset(itemAsset.Id, out SubTemplate subTemplate))
            {
                StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -subTemplate.EnergyAmount);

                ItemSpawnSettings itemSpawnSettings = new()
                {
                    Item = subTemplate.Prototype,
                    Velocity = FPVector2.Zero,
                    Offset = FPVector2.Zero,
                    StartHolding = true
                };

                ItemSpawnSystem.SpawnParented(f, itemSpawnSettings, filter.Entity);
            }
        }

        public override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (filter.CharacterController->DirectionEnum == Direction.Neutral)
            {
                filter.CharacterController->DirectionEnum = DirectionalHelper.GetEnumFromDirection(new(filter.CharacterController->MovementDirection, 0));
            }
            if (filter.CharacterController->DirectionValue == FPVector2.Zero)
            {
                filter.CharacterController->DirectionValue = new(filter.CharacterController->MovementDirection, 0);
            }

            ItemSystem.Throw(f, filter.Entity, filter.PlayerStats->HeldItem, DirectionalHelper.GetFromDirection(settings.ThrowOffset, filter.CharacterController->DirectionEnum), filter.CharacterController->DirectionValue * settings.ThrowForce + settings.ThrowForceOffset);

            base.Exit(f, ref filter, input, settings);
        }
    }
}
