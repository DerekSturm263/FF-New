using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class EmoteState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Emote;

        public override (States, StatesFlag) GetStateInfo() => (States.Emote, StatesFlag.Emote);
        public override EntranceType GetEntranceType() => EntranceType.Grounded;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dead, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Knockback, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Burst, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Emote, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Block, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: false, overrideEnter: false)
        ];

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

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
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
