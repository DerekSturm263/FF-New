using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class EmoteState : DirectionalActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Emote;

        public override (States, StatesFlag) GetStateInfo() => (States.Emote, StatesFlag.Emote);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Burst },
            new() { Destination = States.Dodge },
            new() { Destination = States.Emote },
            new() { Destination = States.Interact },
            new() { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new() { Destination = States.Sub },
            new() { Destination = States.Ultimate },
            new() { Destination = States.Block },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            EmoteMessageBinding emoteAsset = DirectionalHelper.GetFromDirection(filter.PlayerStats->Build.Emotes, filter.CharacterController->DirectionEnum);
            if (f.TryFindAsset(emoteAsset.Emote.Id, out Emote emote))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(emote.Animation.Id);

                if (animEvent.AnimID != 0)
                    return CustomAnimator.GetStateLength(f, filter.CustomAnimator, animEvent.AnimID);
            }

            return 0;
        }

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (!base.CanEnter(f, ref filter, input, settings))
                return false;

            EmoteMessageBinding emoteAsset = DirectionalHelper.GetFromDirection(filter.PlayerStats->Build.Emotes, filter.CharacterController->DirectionEnum);
            return f.TryFindAsset(emoteAsset.Emote.Id, out Emote emote) && emote.Animation.Id.IsValid;
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            EmoteMessageBinding emoteAsset = DirectionalHelper.GetFromDirection(filter.PlayerStats->Build.Emotes, filter.CharacterController->DirectionEnum);
            if (f.TryFindAsset(emoteAsset.Emote.Id, out Emote emote))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(emote.Animation.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);

                filter.CharacterController->PossibleStates = 0;
            }

            if (f.TryFindAsset(emoteAsset.Message.Id, out MessagePreset message))
            {
                f.Events.OnSendMessage(filter.Entity, filter.PlayerStats->Index, message.Message);
            }
        }
    }
}
