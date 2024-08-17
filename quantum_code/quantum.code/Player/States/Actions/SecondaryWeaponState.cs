using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class SecondaryWeaponState : DirectionalActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.AlternateWeapon;

        public override (States, StatesFlag) GetStateInfo() => (States.Secondary, 0);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Burst },
            new() { Destination = States.Dodge },
            new() { Destination = States.Emote },
            new() { Destination = States.Interact },
            new() { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new() { Destination = States.Sub },
            new() { Destination = States.Ultimate },
            new() { Destination = States.Block },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
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

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (!base.CanEnter(f, ref filter, input, settings))
                return false;

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.AltWeapon;
            if (!f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
                return false;

            MoveRef animRef = DirectionalHelper.GetFromDirection(weaponTemplate.Secondaries, filter.CharacterController->DirectionEnum);
            return f.TryFindAsset(animRef.Animation.Id, out QuantumAnimationEvent animEvent) && animEvent.AnimID != 0;
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

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

        public override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            filter.CharacterController->PossibleStates = (StatesFlag)511;
            filter.PlayerStats->ActiveWeapon = EntityRef.None;

            base.Exit(f, ref filter, input, settings);
        }
    }
}
