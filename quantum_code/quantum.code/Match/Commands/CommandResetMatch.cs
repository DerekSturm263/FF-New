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

            TimerSystem.SetTime(f, new(0, 0, f.Global->CurrentMatch.Ruleset.Match.Time), false);
            TimerSystem.StopCountdown(f);

            StatsSystem.SetAllHealth(f, 0);
            StatsSystem.SetAllEnergy(f, 0);
            StatsSystem.SetAllStocks(f, 0);
            StatsSystem.ResetAllTemporaryValues(f);

            f.Global->CanPlayersEdit = true;
            f.Global->IsTimerOver = false;
            f.Global->PlayersReady = 0;
        }
    }
}
