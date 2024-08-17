using Quantum.Custom.Animator;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class MainWeaponState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsUsingMainWeapon, 0);

        public override bool GetInput(ref Input input) => input.MainWeapon;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            Log.Debug("Tyring to get the time...");

            Weapon mainWeaponAsset = filter.PlayerStats->Build.Gear.MainWeapon;

            if (f.TryFindAsset(mainWeaponAsset.Template.Id, out WeaponTemplate mainWeapon))
            {
                Log.Debug("Found asset");

                MoveRef animRef;

                if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                    animRef = DirectionalHelper.GetFromDirection(mainWeapon.Primaries, filter.CharacterController->Direction);
                else
                    animRef = DirectionalHelper.GetFromDirection(mainWeapon.Aerials, filter.CharacterController->Direction);

                if (f.TryFindAsset(animRef.Step1.Id, out QuantumAnimationEvent animEvent) && animEvent.AnimID != 0)
                {
                    int frameCount = (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, animEvent.AnimID).motion as AnimatorClip).data.frameCount;
                    Log.Debug(frameCount);

                    return frameCount;
                }
            }

            Log.Debug("Didn't find asset or it didn't have an animation");

            return 1;
        }
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;
        public override bool CanInterruptSelf => true;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return !filter.CharacterController->IsCommitted;
        }

        public override States[] EntranceBlacklist => new States[] { States.IsInteracting, States.IsBlocking, States.IsDodging, States.IsBursting };

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Direction = DirectionalHelper.GetEnumFromDirection(input.Movement);
            filter.PlayerStats->ActiveWeapon = filter.PlayerStats->MainWeapon;

            Weapon mainWeaponAsset = filter.PlayerStats->Build.Gear.MainWeapon;
            if (f.TryFindAsset(mainWeaponAsset.Template.Id, out WeaponTemplate mainWeapon))
            {
                MoveRef animRef;

                if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                    animRef = DirectionalHelper.GetFromDirection(mainWeapon.Primaries, filter.CharacterController->Direction);
                else
                    animRef = DirectionalHelper.GetFromDirection(mainWeapon.Aerials, filter.CharacterController->Direction);

                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(animRef.Step1.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);

                filter.CharacterController->PossibleStates = 0;
            }
        }

        protected override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Exit(f, ref filter, ref input, settings, stats);

            filter.CharacterController->PossibleStates = (StatesFlag)511;
            filter.PlayerStats->ActiveWeapon = EntityRef.None;
        }
    }
}
