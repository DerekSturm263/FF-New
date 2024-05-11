﻿using Quantum.Custom.Animator;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class EmoteState : PlayerState
    {
        public override States GetState() => States.IsEmoting;

        public override bool GetInput(ref Input input) => input.Emote;
        public override StateType GetStateType() => StateType.Grounded;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            AssetRefEmote emoteAsset = DirectionalAssetHelper.GetFromDirection(filter.Stats->Build.Cosmetics.Emotes, filter.CharacterController->Direction);

            if (f.TryFindAsset(emoteAsset.Id, out Emote emote))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(emote.Animation.MoveAnim.Id);

                if (animEvent.AnimID != 0)
                    return (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, animEvent.AnimID).motion as AnimatorClip).data.frameCount;
            }

            return 1;
        }
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);
        }

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

            AssetRefEmote emoteAsset = DirectionalAssetHelper.GetFromDirection(filter.Stats->Build.Cosmetics.Emotes, filter.CharacterController->Direction);
            if (f.TryFindAsset(emoteAsset.Id, out Emote emote))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(emote.Animation.MoveAnim.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);
            }
        }
    }
}