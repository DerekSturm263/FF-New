using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class EnergyConversionBadge : Badge
    {
        public AssetRefPlayerState Block;

        public FP HealthGain;
        public FP EnergyLoss;

        public override void OnUpdate(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController) &&
                characterController->CurrentState == Block &&
                f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                Behavior behavior = f.FindAsset<Behavior>(characterController->Behavior.Id);
                Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(user).Player) : behavior.GetInput(f, default);
                
                if (!characterController->IsHeldThisFrame(input, Input.Buttons.MainWeapon))
                    return;

                if (stats->CurrentStats.Energy > 0 && stats->CurrentStats.Health < f.Global->CurrentMatch.Ruleset.Players.MaxHealth)
                {
                    StatsSystem.ModifyHealth(f, user, stats, HealthGain, true);
                    StatsSystem.ModifyEnergy(f, user, stats, -EnergyLoss);
                }
            }
        }
    }
}
