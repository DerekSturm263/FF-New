using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ConfirmationSystem : SystemMainThreadFilter<ConfirmationSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public CharacterController* CharacterController;
            public PlayerLink* PlayerLink;
            public Stats* Stats;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            // Get the player's input before we do anything with it.
            Input input = *f.GetPlayerInput(filter.PlayerLink->Player);

            // Set the player's readiness.
            HandleReady(f, ref filter, ref input, filter.PlayerLink);
            HandleCancel(f, ref filter, ref input, filter.PlayerLink);
        }

        private void HandleReady(Frame f, ref Filter filter, ref Input input, PlayerLink* playerLink)
        {
            if (input.Ready && !filter.CharacterController->IsReady)
                filter.CharacterController->ReadyTime += f.DeltaTime;
            else
                filter.CharacterController->ReadyTime = 0;

            f.Events.OnPlayerUpdateReady(filter.Entity, filter.Stats->PlayerIndex, filter.CharacterController->ReadyTime / FP._0_50);

            if (filter.CharacterController->ReadyTime > FP._0_50)
            {
                filter.CharacterController->IsReady = true;

                if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
                {
                    if (!playerCounter->CanPlayersEdit)
                        return;

                    ++playerCounter->PlayersReady;
                    f.Events.OnPlayerReady(filter.Entity, filter.Stats->PlayerIndex);

                    if (playerCounter->TotalPlayers > 1 && playerCounter->PlayersReady == playerCounter->TotalPlayers)
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

                    bool shouldCancelAll = playerCounter->PlayersReady == playerCounter->TotalPlayers;

                    --playerCounter->PlayersReady;

                    filter.CharacterController->IsReady = false;
                    f.Events.OnPlayerCancel(filter.Entity, filter.Stats->PlayerIndex);
                    
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
                TimerSystem.SetTime(f, new(0, 0, matchInstance->Match.Ruleset.Match.Time), false);
            }

            StatsSystem.SetAllHealth(f, 0);
            StatsSystem.SetAllEnergy(f, 0);
            StatsSystem.SetAllStocks(f, 0);

            f.Events.OnAllPlayersCancel();
        }
    }
}
