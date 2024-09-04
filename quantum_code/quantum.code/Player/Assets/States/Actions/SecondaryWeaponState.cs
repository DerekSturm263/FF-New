using Quantum.Types;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class SecondaryWeaponState : ActionState
    {
        public AssetRefPlayerState Default;

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

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.PlayerStats->ActiveWeapon = ActiveWeaponType.Secondary;

            Weapon altWeaponAsset = filter.PlayerStats->Build.Gear.AltWeapon;
            if (f.TryFindAsset(altWeaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
            {
                MoveRef animRef = DirectionalHelper.GetFromDirection(weaponTemplate.Secondaries, filter.CharacterController->DirectionEnum);
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(animRef.Animation.Id);

                WeaponMaterial material = f.FindAsset<WeaponMaterial>(filter.PlayerStats->Build.Gear.AltWeapon.Material.Id);
                if (material is not null)
                    filter.CustomAnimator->speed = material.Stats.Speed;

                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);
            }

            filter.CharacterController->HoldLevel = 0;
        }

        public override void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.PlayerStats->ActiveWeapon = ActiveWeaponType.None;

            filter.CharacterController->HeldAnimationFrameTime = 0;
            filter.CharacterController->MaxHoldAnimationFrameTime = 0;

            base.FinishExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
