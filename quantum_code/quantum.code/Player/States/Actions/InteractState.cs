using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class InteractState : DirectionalActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Interact;

        public override (States, StatesFlag) GetStateInfo() => (States.Interact, StatesFlag.Interact);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

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

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => settings.InteractTime;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, ref filter, input, settings) &&
                (filter.PlayerStats->HeldItem.IsValid ||
                settings.InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0) * settings.InteractCastDistanceMultiplier).Count > 0);
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            filter.CharacterController->Velocity = 0;
            
            if (filter.CharacterController->DirectionEnum == Direction.Neutral)
            {
                filter.CharacterController->DirectionEnum = DirectionalHelper.GetEnumFromDirection(new(filter.CharacterController->MovementDirection, 0));
            }
            if (filter.CharacterController->DirectionValue == FPVector2.Zero)
            {
                filter.CharacterController->DirectionValue = new(filter.CharacterController->MovementDirection, 0);
            }

            if (!filter.PlayerStats->HeldItem.IsValid)
            {
                Physics2D.HitCollection hitCollection = settings.InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0));

                for (int i = 0; i < hitCollection.Count; ++i)
                {
                    Log.Debug(hitCollection[i].Entity.Index);

                    if (f.Unsafe.TryGetPointer(hitCollection[i].Entity, out ItemInstance* itemInstance))
                    {
                        ItemSystem.Use(f, filter.Entity, hitCollection[i].Entity, itemInstance);
                        break;
                    }
                }
            }
            else
            {
                ItemSystem.Throw(f, filter.Entity, filter.PlayerStats->HeldItem, DirectionalHelper.GetFromDirection(settings.ThrowOffset, filter.CharacterController->DirectionEnum), filter.CharacterController->DirectionValue * settings.ThrowForce + settings.ThrowForceOffset);
            }
        }
    }
}
