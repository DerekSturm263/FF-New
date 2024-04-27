using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class MainWeaponState : PlayerState
    {
        public override States GetState() => States.IsUsingMainWeapon;

        public override bool GetInput(ref Input input) => input.MainWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => -1;
        protected override int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => settings.DirectionChangeTime;

        protected override bool CanExit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CustomAnimator->normalized_time == 1;
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);
        }

        protected override void DelayedEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.DelayedEnter(f, ref filter, ref input, settings);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

            AssetRefMainWeapon mainWeaponAsset = filter.Stats->Build.Equipment.WeaponSettings.MainWeapon;
            if (f.TryFindAsset(mainWeaponAsset.Id, out MainWeapon mainWeapon))
            {
                AnimationRef animRef;

                if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                    animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.Grounded, filter.CharacterController->Direction);
                else
                    animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.Aerial, filter.CharacterController->Direction);

                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animRef.ID);
            }
        }
    }
}
