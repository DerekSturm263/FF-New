using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ConfirmationSystem : SystemMainThreadFilter<ConfirmationSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController* CharacterController;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            Input input = default;
            PlayerRef playerRef = default;

            if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
            {
                playerRef = playerLink->Player;
                input = *f.GetPlayerInput(playerRef);

                HandleReady(f, ref filter, ref input, playerLink);
                HandleCancel(f, ref filter, ref input, playerLink);
            }
        }

        private void HandleReady(Frame f, ref Filter filter, ref Input input, PlayerLink* playerLink)
        {
            if (input.Ready && !filter.CharacterController->IsReady)
                filter.CharacterController->ReadyTime += f.DeltaTime;
            else
                filter.CharacterController->ReadyTime = 0;

            if (filter.CharacterController->ReadyTime > FP._0_50)
            {
                filter.CharacterController->IsReady = true;

                if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
                {
                    if (!playerCounter->CanPlayersEdit)
                        return;

                    ++playerCounter->PlayersReady;
                    f.Events.OnPlayerReady(*playerLink);

                    if (f.PlayerCount > 1 && playerCounter->PlayersReady == f.PlayerCount)
                    {
                        HandleAllPlayersReady(f);
                    }
                }
            }
        }

        private void HandleCancel(Frame f, ref Filter filter, ref Input input, PlayerLink* playerLink)
        {
            if (input.Cancel && filter.CharacterController->IsReady)
            {
                if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
                {
                    if (!playerCounter->CanPlayersEdit)
                        return;

                    bool shouldCancelAll = playerCounter->PlayersReady == f.PlayerCount;

                    --playerCounter->PlayersReady;

                    filter.CharacterController->IsReady = false;
                    f.Events.OnPlayerCancel(*playerLink);
                    
                    if (shouldCancelAll)
                        HandleAllPlayersCancel(f);
                }
            }
        }

        private void HandleAllPlayersReady(Frame f)
        {
            f.Events.OnAllPlayersReady();

            f.SystemDisable<CharacterControllerSystem>();
            TimerSystem.ResumeCountdown(f);

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                TimerSystem.SetTime(f, new(0, 0, matchInstance->Match.Ruleset.Match.Time + 3));
            }
        }

        private void HandleAllPlayersCancel(Frame f)
        {
            f.SystemEnable<CharacterControllerSystem>();
            TimerSystem.StopCountdown(f);

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                TimerSystem.SetTime(f, new(0, 0, matchInstance->Match.Ruleset.Match.Time));
            }

            var filter = f.Unsafe.FilterStruct<StatsSystem.PlayerLinkStatsFilter>();
            var playerLinkStats = default(StatsSystem.PlayerLinkStatsFilter);

            while (filter.Next(&playerLinkStats))
            {
                StatsSystem.SetHealth(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, 0);
                StatsSystem.SetEnergy(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, 0);
                StatsSystem.SetStocks(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, 0);
            }

            f.Events.OnAllPlayersCancel();
        }
    }
}
