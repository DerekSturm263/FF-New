using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class SubState : InputState
    {
        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (!base.CanEnter(f, stateMachine, ref filter, input, settings) || filter.PlayerStats->HeldItem.IsValid)
                return false;

            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.SubWeapon.Template.Id, out SubTemplate subWeapon))
                return subWeapon.Prototype.Id.IsValid && filter.Stats->CurrentStats.Energy >= subWeapon.EnergyAmount;

            return false;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
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

                EntityRef item = ItemSpawnSystem.SpawnParented(f, itemSpawnSettings, filter.Entity);

                SubEnhancer subEnhancer = f.FindAsset<SubEnhancer>(filter.PlayerStats->Build.Gear.SubWeapon.Enhancer.Id);
                subEnhancer?.OnSpawn(f, filter.Entity, item, filter.PlayerStats->Build.Gear.SubWeapon);

                filter.CharacterController->HasSubWeapon = true;
                ++filter.PlayerStats->Stats.SubUses;
            }
        }
    }
}
