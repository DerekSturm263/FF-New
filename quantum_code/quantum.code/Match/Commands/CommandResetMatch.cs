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

            var filter = f.Unsafe.FilterStruct<StatsSystem.PlayerLinkStatsFilter>();
            var playerLinkStats = default(StatsSystem.PlayerLinkStatsFilter);

            while (filter.Next(&playerLinkStats))
            {
                StatsSystem.SetHealth(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, 0);
                StatsSystem.SetEnergy(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, 0);
                StatsSystem.SetStocks(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, 0);
            }

            if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
            {
                playerCounter->CanPlayersEdit = true;
            }
        }
    }
}
