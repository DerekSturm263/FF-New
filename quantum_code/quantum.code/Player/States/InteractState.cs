﻿using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class InteractState : PlayerState
    {
        public override States GetState() => States.IsInteracting;

        public override bool GetInput(ref Input input) => input.Interact;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => 1;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.Stats->HeldItem.IsValid || settings.InteractCast.GetCastResults(f, filter.Transform).Count > 0;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            if (!filter.Stats->HeldItem.IsValid)
            {
                Physics2D.HitCollection hitCollection = settings.InteractCast.GetCastResults(f, filter.Transform);

                for (int i = 0; i < hitCollection.Count; ++i)
                {
                    if (f.Unsafe.TryGetPointer(hitCollection[i].Entity, out ItemInstance* itemInstance))
                    {
                        ItemSystem.Use(f, filter.PlayerLink, hitCollection[i].Entity, itemInstance);
                        break;
                    }
                }
            }
            else
            {
                ItemSystem.Throw(f, filter.Entity, filter.Stats->HeldItem, DirectionalAssetHelper.GetFromDirection(settings.ThrowForce, filter.CharacterController->Direction));
            }
        }
    }
}
