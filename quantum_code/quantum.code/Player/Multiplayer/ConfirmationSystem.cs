using Photon.Deterministic;
using Quantum.Movement;
using Quantum.Timing;

namespace Quantum.Multiplayer
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
            {
                if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
                {
                    if (!playerCounter->CanPlayersEdit)
                        return;

                    ++playerCounter->PlayersReady;
                }

                filter.CharacterController->IsReady = true;
                f.Events.OnPlayerReady(*playerLink);

                if (playerCounter is not null && playerCounter->PlayersReady == playerCounter->TotalPlayers)
                {
                    HandleAllPlayersReady(f);
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

                    --playerCounter->PlayersReady;
                }

                filter.CharacterController->IsReady = false;
                f.Events.OnPlayerCancel(*playerLink);

                f.SystemEnable<MovementSystem>();
                f.SystemDisable<TimerSystem>();
            }
        }

        private void HandleAllPlayersReady(Frame f)
        {
            f.Events.OnAllPlayersReady();

            f.SystemDisable<MovementSystem>();
            f.SystemEnable<TimerSystem>();
        }
    }
}
