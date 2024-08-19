using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class SecondaryWeaponState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.AlternateWeapon;

        public override (States, StatesFlag) GetStateInfo() => (States.Secondary, 0);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
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

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug("Tyring to get the time...");

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.AltWeapon;
            if (f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
            {
                Log.Debug("Found asset");

                MoveRef animRef = DirectionalHelper.GetFromDirection(weaponTemplate.Secondaries, filter.CharacterController->DirectionEnum);
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

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.AltWeapon;
            if (!f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
                return false;

            MoveRef animRef = DirectionalHelper.GetFromDirection(weaponTemplate.Secondaries, filter.CharacterController->DirectionEnum);
            return f.TryFindAsset(animRef.Animation.Id, out QuantumAnimationEvent animEvent) && animEvent.AnimID != 0;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.PlayerStats->ActiveWeapon = filter.PlayerStats->AltWeapon;

            Weapon altWeaponAsset = filter.PlayerStats->Build.Gear.AltWeapon;
            if (f.TryFindAsset(altWeaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
            {
                MoveRef animRef = DirectionalHelper.GetFromDirection(weaponTemplate.Secondaries, filter.CharacterController->DirectionEnum);
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(animRef.Animation.Id);

                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);

                filter.CharacterController->PossibleStates = 0;
            }
        }

        public override void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            filter.CharacterController->PossibleStates = (StatesFlag)511;
            filter.PlayerStats->ActiveWeapon = EntityRef.None;

            base.FinishExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
