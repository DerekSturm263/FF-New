using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class PrimaryWeaponState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.MainWeapon;

        public override (States, StatesFlag) GetStateInfo() => (States.Primary, StatesFlag.Primary);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override bool OverrideDirection() => true;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Dead, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Knockback, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Burst, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Emote, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Block, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: true, overrideEnter: false)
        ];

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug("Tyring to get the time...");

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.MainWeapon;
            if (f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
            {
                Log.Debug("Found asset");

                MoveRef animRef = filter.CharacterController->GetNearbyCollider(Colliders.Ground) ?
                    DirectionalHelper.GetFromDirection(weaponTemplate.Primaries, filter.CharacterController->DirectionEnum) :
                    DirectionalHelper.GetFromDirection(weaponTemplate.Aerials, filter.CharacterController->DirectionEnum);

                if (f.TryFindAsset(animRef.Animation.Id, out QuantumAnimationEvent animEvent) && animEvent.AnimID != 0)
                {
                    int frameCount = CustomAnimator.GetStateLength(f, filter.CustomAnimator, animEvent.AnimID);
                    Log.Debug(frameCount);

                    return frameCount;
                }
            }

            Log.Debug("Didn't find asset or it didn't have an animation");

            return 0;
        }

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (!base.CanEnter(f, stateMachine, ref filter, input, settings))
                return false;

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.MainWeapon;
            if (!f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
                return false;

            MoveRef animRef = DirectionalHelper.GetFromDirection(weaponTemplate.Primaries, filter.CharacterController->DirectionEnum);
            return f.TryFindAsset(animRef.Animation.Id, out QuantumAnimationEvent animEvent) && animEvent.AnimID != 0;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.PlayerStats->ActiveWeapon = ActiveWeaponType.Primary;

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.MainWeapon;
            if (f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
            {
                MoveRef animRef = filter.CharacterController->GetNearbyCollider(Colliders.Ground) ?
                    DirectionalHelper.GetFromDirection(weaponTemplate.Primaries, filter.CharacterController->DirectionEnum) :
                    DirectionalHelper.GetFromDirection(weaponTemplate.Aerials, filter.CharacterController->DirectionEnum);

                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(animRef.Animation.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);
            }
        }

        public override void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            filter.PlayerStats->ActiveWeapon = ActiveWeaponType.None;

            base.FinishExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
