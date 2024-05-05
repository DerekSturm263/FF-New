using Quantum.Custom.Animator;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class AltWeaponState : PlayerState
    {
        public override States GetState() => States.IsUsingAltWeapon;

        public override bool GetInput(ref Input input) => input.AlternateWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            MainWeapon mainWeaponAsset = filter.Stats->Build.Equipment.Weapons.AltWeapon;

            if (f.TryFindAsset(mainWeaponAsset.Template.Id, out MainWeaponTemplate mainWeapon))
            {
                MoveRef animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.GroundedSupports, filter.CharacterController->Direction);
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(animRef.MoveAnim.Id);

                if (animEvent.AnimID != 0)
                    return (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, animEvent.AnimID).motion as AnimatorClip).data.frameCount;
            }

            return 1;
        }
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;
        public override bool CanInterruptSelf => true;

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);
        }

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

            MainWeapon mainWeaponAsset = filter.Stats->Build.Equipment.Weapons.AltWeapon;
            if (f.TryFindAsset(mainWeaponAsset.Template.Id, out MainWeaponTemplate mainWeapon))
            {
                MoveRef animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.GroundedHeavies, filter.CharacterController->Direction);
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(animRef.MoveAnim.Id);

                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);
            }
        }
    }
}
