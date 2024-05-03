using Quantum.Custom.Animator;
using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class EmoteState : PlayerState
    {
        public override States GetState() => States.IsEmoting;

        public override bool GetInput(ref Input input) => input.Emote;
        public override StateType GetStateType() => StateType.Grounded;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            AssetRefEmote emoteAsset = DirectionalAssetHelper.GetFromDirection(filter.Stats->Build.Cosmetics.Emotes, filter.CharacterController->Direction);

            if (f.TryFindAsset(emoteAsset.Id, out Emote emote))
            {
                if (emote.Animation.ID != 0)
                    return (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, emote.Animation.ID).motion as AnimatorClip).data.frameCount;
            }

            return 1;
        }
        protected override int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);
        }

        protected override void DelayedEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

            AssetRefEmote emoteAsset = DirectionalAssetHelper.GetFromDirection(filter.Stats->Build.Cosmetics.Emotes, filter.CharacterController->Direction);
            if (f.TryFindAsset(emoteAsset.Id, out Emote emote))
            {
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, emote.Animation.ID);
            }
        }
    }
}
