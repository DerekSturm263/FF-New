using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class EnergyConversionBadge : Badge
    {
        public AssetRefPlayerState Block;

        public FP HealthGain;
        public FP EnergyLoss;

        public override void OnUpdate(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            if (filter.CharacterController->CurrentState == Block)
            {
                Behavior behavior = f.FindAsset<Behavior>(filter.CharacterController->Behavior.Id);
                Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(filter.Entity).Player) : behavior.GetInput(f, default);
                
                if (!filter.CharacterController->IsHeldThisFrame(input, Input.Buttons.MainWeapon))
                    return;

                if (filter.Stats->CurrentStats.Energy > 0 && filter.Stats->CurrentStats.Health < f.Global->CurrentMatch.Ruleset.Players.MaxHealth)
                {
                    StatsSystem.ModifyHealth(f, filter.Entity, filter.Stats, HealthGain, true);
                    StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -EnergyLoss);
                }
            }
        }
    }
}
