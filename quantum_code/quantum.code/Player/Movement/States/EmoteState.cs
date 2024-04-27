using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class EmoteState : MovementState
    {
        public override States GetState() => States.IsEmoting;

        public override bool GetInput(ref Input input) => input.Emote;
        public override StateType GetStateType() => StateType.Grounded;

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CustomAnimator->normalized_time == 1;
        }

        protected override void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.CharacterController->StartupLag = 1;

            filter.CharacterController->IsReady = false;
            filter.CharacterController->Invoked = false;
        }

        public override void Update(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, ref input, settings);

            if (filter.CharacterController->StartupLag <= filter.CharacterController->AttackOpenLag)
            {
                Direction newDirection = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

                if (filter.CharacterController->Direction != newDirection)
                {
                    filter.CharacterController->Direction = newDirection;
                    filter.CharacterController->IsReady = true;
                }
            }
            else if (filter.CharacterController->StartupLag == filter.CharacterController->AttackOpenLag + 1)
            {
                filter.CharacterController->IsReady = true;
            }

            if (filter.CharacterController->IsReady && !filter.CharacterController->Invoked)
            {
                filter.CharacterController->Invoked = true;

                AssetRefEmote emoteAsset = DirectionalAssetHelper.GetFromDirection(filter.Stats->Build.Cosmetics.Emotes, filter.CharacterController->Direction);
                if (f.TryFindAsset(emoteAsset.Id, out Emote emote))
                {
                    CustomAnimator.SetCurrentState(f, filter.CustomAnimator, emote.Animation.ID);
                }
            }

            ++filter.CharacterController->StartupLag;
        }

        protected override void Exit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, ref input, settings);

            filter.CharacterController->IsReady = false;
            filter.CharacterController->Invoked = false;
        }
    }
}
