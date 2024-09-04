using Photon.Deterministic;
using Quantum.Inspector;
using Quantum.Types;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class InteractState : ActionState
    {
        [Header("State-Specific Values")]

        public ShapeCastHelper InteractCast;
        public FP InteractCastDistanceMultiplier;

        public DirectionalFPVector2 ThrowOffset;
        public FP ThrowForce;
        public FPVector2 ThrowForceOffset;

        public int ThrowTime;
        public int UseTime;

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->IsThrowing ? ThrowTime : UseTime;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) &&
                (filter.PlayerStats->HeldItem.IsValid ||
                filter.CharacterController->HasSubWeapon ||
                InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0) * InteractCastDistanceMultiplier).Count > 0);
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);
            
            if (!filter.PlayerStats->HeldItem.IsValid)
            {
                Physics2D.HitCollection hitCollection = InteractCast.GetCastResults(f, filter.Transform, new FPVector2(filter.CharacterController->MovementDirection, 0));

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

                filter.CharacterController->HasSubWeapon = false;
            }
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

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
                
                ItemSystem.Throw(f, filter.Entity, filter.PlayerStats->HeldItem, DirectionalHelper.GetFromDirection(ThrowOffset, filter.CharacterController->DirectionEnum), (filter.CharacterController->DirectionValue * ThrowForce + ThrowForceOffset) * filter.CharacterController->ThrowMultiplier);

                if (filter.CharacterController->DirectionValue.X != 0)
                {
                    filter.CharacterController->MovementDirection = FPMath.SignInt(filter.CharacterController->DirectionValue.X);
                    f.Events.OnPlayerChangeDirection(filter.Entity, filter.PlayerStats->Index, filter.CharacterController->MovementDirection);
                }
            }
        }
    }
}
