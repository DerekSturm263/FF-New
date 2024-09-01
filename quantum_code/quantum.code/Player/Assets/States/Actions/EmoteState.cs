using Quantum.Types;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class EmoteState : ActionState
    {
        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            EmoteMessageBinding emoteAsset = DirectionalHelper.GetFromDirection(filter.PlayerStats->Build.Emotes, filter.CharacterController->DirectionEnum);
            if (f.TryFindAsset(emoteAsset.Emote.Id, out Emote emote))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(emote.Animation.Id);

                if (animEvent is not null && animEvent.AnimID != 0)
                    return CustomAnimator.GetStateLength(f, filter.CustomAnimator, animEvent.AnimID);
            }

            return 0;
        }

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (!base.CanEnter(f, stateMachine, ref filter, input, settings))
                return false;

            EmoteMessageBinding emoteAsset = DirectionalHelper.GetFromDirection(filter.PlayerStats->Build.Emotes, filter.CharacterController->DirectionEnum);
            return f.TryFindAsset(emoteAsset.Emote.Id, out Emote emote) && emote.Animation.Id.IsValid;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            EmoteMessageBinding emoteAsset = DirectionalHelper.GetFromDirection(filter.PlayerStats->Build.Emotes, filter.CharacterController->DirectionEnum);
            if (f.TryFindAsset(emoteAsset.Emote.Id, out Emote emote))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(emote.Animation.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);
            }

            if (f.TryFindAsset(emoteAsset.Message.Id, out MessagePreset message))
            {
                f.Events.OnSendMessage(filter.Entity, filter.PlayerStats->Index, message.Message);
            }
        }
    }
}
