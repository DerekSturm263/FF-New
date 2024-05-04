using Quantum.Custom.Animator;
using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class MainWeaponState : PlayerState
    {
        public override States GetState() => States.IsUsingMainWeapon;

        public override bool GetInput(ref Input input) => input.MainWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            MainWeapon mainWeaponAsset = filter.Stats->Build.Equipment.Weapons.MainWeapon;

            if (f.TryFindAsset(mainWeaponAsset.Template.Id, out MainWeaponTemplate mainWeapon))
            {
                AnimationRef animRef;

                if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                    animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.GroundedHeavies, filter.CharacterController->Direction);
                else
                    animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.Aerials, filter.CharacterController->Direction);

                if (animRef.ID != 0)
                    return (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, animRef.ID).motion as AnimatorClip).data.frameCount;
            }

            return 1;
        }
        protected override int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;
        public override bool CanInterruptSelf => true;

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);
        }

        protected override void DelayedEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

            MainWeapon mainWeaponAsset = filter.Stats->Build.Equipment.Weapons.MainWeapon;
            if (f.TryFindAsset(mainWeaponAsset.Template.Id, out MainWeaponTemplate mainWeapon))
            {
                AnimationRef animRef;

                if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                    animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.GroundedHeavies, filter.CharacterController->Direction);
                else
                    animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.Aerials, filter.CharacterController->Direction);

                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animRef.ID);
            }
        }
    }
}
