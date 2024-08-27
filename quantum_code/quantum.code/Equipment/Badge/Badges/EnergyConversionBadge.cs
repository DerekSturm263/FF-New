using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class EnergyConversionBadge : Badge
    {
        public FP HealthGain;
        public FP EnergyLoss;

        public override void OnUpdate(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController) &&
                characterController->CurrentState == States.Block &&
                f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                if (stats->CurrentStats.Energy > 0 && stats->CurrentStats.Health < f.Global->CurrentMatch.Ruleset.Players.MaxHealth)
                {
                    StatsSystem.ModifyHealth(f, user, stats, HealthGain, true);
                    StatsSystem.ModifyEnergy(f, user, stats, -EnergyLoss);
                }
            }
        }
    }
}
