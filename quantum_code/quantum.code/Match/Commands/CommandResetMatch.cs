using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandResetMatch : DeterministicCommand
    {
        public override void Serialize(BitStream stream)
        {

        }

        public void Execute(Frame f)
        {
            Log.Debug("Match reset!");

            f.SystemEnable<CharacterControllerSystem>();

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                TimerSystem.SetTime(f, new(0, 0, matchInstance->Match.Ruleset.Match.Time), false);
                TimerSystem.StopCountdown(f);
            }

            StatsSystem.SetAllHealth(f, 0);
            StatsSystem.SetAllEnergy(f, 0);
            StatsSystem.SetAllStocks(f, 0);

            if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
            {
                playerCounter->CanPlayersEdit = true;
            }
        }
    }
}
