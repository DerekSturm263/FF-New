using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class PrimaryWeaponState : ActionState
    {
        public FP MinimumDashAttackSpeed;

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug("Tyring to get the time...");

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.MainWeapon;
            if (f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
            {
                Log.Debug("Found asset");

                MoveRef animRef = filter.CharacterController->GetNearbyCollider(Colliders.Ground) ?
                    FPMath.Abs(filter.CharacterController->Velocity) > MinimumDashAttackSpeed ? weaponTemplate.Dash :
                    DirectionalHelper.GetFromDirection(weaponTemplate.Primaries, filter.CharacterController->DirectionEnum) :
                    DirectionalHelper.GetFromDirection(weaponTemplate.Aerials, filter.CharacterController->DirectionEnum, filter.CharacterController->MovementDirection == 1);

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

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.PlayerStats->ActiveWeapon = ActiveWeaponType.Primary;

            Weapon weaponAsset = filter.PlayerStats->Build.Gear.MainWeapon;
            if (f.TryFindAsset(weaponAsset.Template.Id, out WeaponTemplate weaponTemplate))
            {
                MoveRef animRef = filter.CharacterController->GetNearbyCollider(Colliders.Ground) ?
                    DirectionalHelper.GetFromDirection(weaponTemplate.Primaries, filter.CharacterController->DirectionEnum) :
                    DirectionalHelper.GetFromDirection(weaponTemplate.Aerials, filter.CharacterController->DirectionEnum, filter.CharacterController->MovementDirection == 1);

                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(animRef.Animation.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);
            }
        }

        public override void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.PlayerStats->ActiveWeapon = ActiveWeaponType.None;

            base.FinishExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
