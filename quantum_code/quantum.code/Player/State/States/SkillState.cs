using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class SkillState : PlayerState
    {
        public override States GetState() => States.IsUsingSkill;

        public override bool GetInput(ref Input input) => input.Skill;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => -1;
        protected override int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => settings.DirectionChangeTime;
        public override bool CanInterruptSelf => true;

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

            AssetRefSkill skillAsset = DirectionalAssetHelper.GetFromDirection(filter.Stats->Build.Equipment.Skills, filter.CharacterController->Direction);
            if (f.TryFindAsset(skillAsset.Id, out Skill skill))
            {
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, skill.Move.ID);
            }
        }
    }
}
