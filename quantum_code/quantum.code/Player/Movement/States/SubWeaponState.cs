using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class SubWeaponState : PlayerState
    {
        public override States GetState() => States.IsUsingSubWeapon;

        public override bool GetInput(ref Input input) => input.SubWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => -1;
        protected override int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => settings.DirectionChangeTime;

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            if (f.TryFindAsset(filter.Stats->Build.Equipment.WeaponSettings.SubWeapon.Id, out SubWeapon subWeapon))
                return filter.Stats->CurrentEnergy >= subWeapon.EnergyAmount;

            return false;
        }

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
        }
    }
}
