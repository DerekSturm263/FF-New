using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class InteractState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsInteracting, StatesFlag.Interact);

        public override bool GetInput(ref Input input) => input.Interact;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime + 2;
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime * 2;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.PlayerStats->HeldItem.IsValid || settings.InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0) * settings.InteractCastDistanceMultiplier).Count > 0;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Velocity = 0;
        }

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalHelper.GetEnumFromDirection(input.Movement);
            if (filter.CharacterController->Direction == Direction.Neutral)
            {
                filter.CharacterController->Direction = DirectionalHelper.GetEnumFromDirection(new FPVector2(filter.CharacterController->MovementDirection, 0) * settings.InteractCastDistanceMultiplier);
            }

            if (!filter.PlayerStats->HeldItem.IsValid)
            {
                Physics2D.HitCollection hitCollection = settings.InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0));

                for (int i = 0; i < hitCollection.Count; ++i)
                {
                    if (f.Unsafe.TryGetPointer(hitCollection[i].Entity, out ItemInstance* itemInstance))
                    {
                        ItemSystem.Use(f, filter.Entity, hitCollection[i].Entity, itemInstance);
                        break;
                    }
                }
            }
            else
            {
                ItemSystem.Throw(f, filter.Entity, filter.PlayerStats->HeldItem, DirectionalHelper.GetFromDirection(settings.ThrowOffset, filter.CharacterController->Direction), DirectionalHelper.GetFromDirection(settings.ThrowForce, filter.CharacterController->Direction));
            }
        }
    }
}
