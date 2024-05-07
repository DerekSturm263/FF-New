using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class EnergyConversionBadge : Badge
    {
        public override void OnUpdate(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance) &&
                f.Unsafe.TryGetPointer(user, out CharacterController* characterController) &&
                f.Unsafe.TryGetPointer(user, out PlayerLink* playerLink) &&
                f.Unsafe.TryGetPointer(user, out Stats* stats) &&
                characterController->IsInState(States.IsBlocking))
            {
                if (stats->CurrentEnergy > 0 && stats->CurrentHealth < matchInstance->Match.Ruleset.Players.MaxHealth)
                {
                    StatsSystem.ModifyHealth(f, playerLink, stats, 1);
                    StatsSystem.ModifyEnergy(f, playerLink, stats, -1);
                }
            }
        }
    }
}
