using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class InteractState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Interact;

        public override (States, StatesFlag) GetStateInfo() => (States.Interact, StatesFlag.Interact);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
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

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->IsThrowing ? settings.ThrowTime : settings.UseTime;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, ref filter, input, settings) &&
                (filter.PlayerStats->HeldItem.IsValid ||
                settings.InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0) * settings.InteractCastDistanceMultiplier).Count > 0);
        }

        public override void FinishEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, ref filter, input, settings, previousState);

            filter.CharacterController->Velocity = 0;
            
            if (!filter.PlayerStats->HeldItem.IsValid)
            {
                Physics2D.HitCollection hitCollection = settings.InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0));

                for (int i = 0; i < hitCollection.Count; ++i)
                {
                    Log.Debug(hitCollection[i].Entity.Index);

                    if (f.Unsafe.TryGetPointer(hitCollection[i].Entity, out ItemInstance* itemInstance))
                    {
                        ItemSystem.Use(f, filter.Entity, hitCollection[i].Entity, itemInstance);

                        filter.CharacterController->IsThrowing = false;
                        CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsThrowing", false);

                        break;
                    }
                }
            }
            else
            {
                filter.CharacterController->IsThrowing = true;
                CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsThrowing", true);
            }
        }

        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, input, settings);

            if (filter.CharacterController->IsThrowing && filter.CharacterController->StateTime == 12)
            {
                if (filter.CharacterController->DirectionEnum == Direction.Neutral)
                {
                    filter.CharacterController->DirectionEnum = DirectionalHelper.GetEnumFromDirection(new(filter.CharacterController->MovementDirection, 0));
                }
                if (filter.CharacterController->DirectionValue == FPVector2.Zero)
                {
                    filter.CharacterController->DirectionValue = new(filter.CharacterController->MovementDirection, 0);
                }
                
                ItemSystem.Throw(f, filter.Entity, filter.PlayerStats->HeldItem, DirectionalHelper.GetFromDirection(settings.ThrowOffset, filter.CharacterController->DirectionEnum), filter.CharacterController->DirectionValue * settings.ThrowForce + settings.ThrowForceOffset);

                if (filter.CharacterController->DirectionValue.X != 0)
                {
                    filter.CharacterController->MovementDirection = FPMath.SignInt(filter.CharacterController->DirectionValue.X);
                    f.Events.OnPlayerChangeDirection(filter.Entity, filter.PlayerStats->Index, filter.CharacterController->MovementDirection);
                }
            }
        }
    }
}
