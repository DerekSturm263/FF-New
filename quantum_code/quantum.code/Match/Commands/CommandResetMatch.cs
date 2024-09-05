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

            TimerSystem.SetTime(f, new(0, 0, f.Global->CurrentMatch.Ruleset.Match.Time), false);
            TimerSystem.StopCountdown(f);

            StatsSystem.SetAllHealth(f, 0);
            StatsSystem.SetAllEnergy(f, 0);
            StatsSystem.SetAllStocks(f, 0);

            PlayerStatsSystem.ResetAllTemporaryValues(f);
            StatsSystem.ResetAllTemporaryValues(f);

            f.Global->CanPlayersEdit = true;
            f.Global->IsTimerOver = false;
            f.Global->PlayersReady = 0;

            var filter = f.Unsafe.FilterStruct<CharacterControllerSystem.Filter>();
            var player = default(CharacterControllerSystem.Filter);

            while (filter.Next(&player))
            {
                MovementSettings settings = f.FindAsset<MovementSettings>(player.CharacterController->Settings.Id);
                CharacterControllerSystem.StateMachine.ForceTransition(f, ref player, default, settings, f.RuntimeConfig.DefaultState, 0);

                player.Stats->IsRespawning = false;

                StatsSystem.SetEnergy(f, player.Entity, player.Stats, 0);
                StatsSystem.ModifyHurtboxes(f, player.Entity, (HurtboxType)((int)HurtboxType.Head * 2 - 1), HurtboxSettings.Default, false);
            }

            PlayerStatsSystem.SetAllReadiness(f, false);
        }
    }
}
