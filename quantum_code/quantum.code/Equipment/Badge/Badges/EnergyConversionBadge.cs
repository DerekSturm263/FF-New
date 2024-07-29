using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class EnergyConversionBadge : Badge
    {
        public override void OnUpdate(Frame f, EntityRef user)
        {
            if (                f.Unsafe.TryGetPointer(user, out CharacterController* characterController) &&
                f.Unsafe.TryGetPointer(user, out Stats* stats) &&
                characterController->IsInState(States.IsBlocking))
            {
                if (stats->CurrentStats.Energy > 0 && stats->CurrentStats.Health < f.Global->CurrentMatch.Ruleset.Players.MaxHealth)
                {
                    StatsSystem.ModifyHealth(f, user, stats, 1, true);
                    StatsSystem.ModifyEnergy(f, user, stats, -1);
                }
            }
        }
    }
}
