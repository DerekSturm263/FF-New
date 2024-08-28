using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class SubState : ExclusivePassiveState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.SubWeapon;

        public override (States, StatesFlag) GetStateInfo() => (States.Sub, StatesFlag.Sub);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dead, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Knockback, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Burst, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Emote, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: true),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Block, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: true, overrideEnter: true)
        ];

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (!base.CanEnter(f, stateMachine, ref filter, input, settings) || filter.PlayerStats->HeldItem.IsValid)
                return false;

            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.SubWeapon.Template.Id, out SubTemplate subWeapon))
                return subWeapon.Prototype.Id.IsValid && filter.Stats->CurrentStats.Energy >= subWeapon.EnergyAmount;

            return false;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

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
                
                filter.CharacterController->HasSubWeapon = true;
                ++filter.PlayerStats->Stats.SubUses;
            }
        }
    }
}
