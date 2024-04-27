using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class AttackState : MovementState
    {
        public override States GetState() => States.IsAttacking;

        public override bool GetInput(ref Input input) => input.MainWeapon || input.SubWeapon || input.Skill || input.Ultimate;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CustomAnimator->normalized_time == 1;
        }

        protected override void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.CharacterController->StartupLag = 1;

            filter.CharacterController->AttackType =
                  input.Ultimate ? AttackType.Ultimate
                : input.MainWeapon ? AttackType.MainWeapon
                : input.SubWeapon ? AttackType.SubWeapon
                : AttackType.Skill;

            if (filter.CharacterController->AttackType != AttackType.Ultimate)
                filter.CharacterController->IsReady = false;

            filter.CharacterController->Invoked = false;

            if (filter.CharacterController->AttackType == AttackType.Ultimate)
            {
                if (filter.Stats->CurrentEnergy < filter.Stats->MaxEnergy)
                    ForceExit(f, ref filter, ref input, settings);
            }

            if (filter.CharacterController->AttackType == AttackType.SubWeapon)
            {
                if (f.TryFindAsset(filter.Stats->Build.Equipment.WeaponSettings.SubWeapon.Id, out SubWeapon subWeapon))
                {
                    if (filter.Stats->CurrentEnergy < subWeapon.EnergyAmount)
                        ForceExit(f, ref filter, ref input, settings);
                }
            }
        }

        public override void Update(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, ref input, settings);

            if (filter.CharacterController->AttackType != AttackType.Ultimate)
            {
                if (!filter.CharacterController->IsReady)
                {
                    if (filter.CharacterController->StartupLag <= filter.CharacterController->AttackOpenLag)
                    {
                        Direction newDirection = DirectionalAssetHelper.GetEnumFromDirection(input.Movement);

                        if (filter.CharacterController->Direction != newDirection)
                        {
                            filter.CharacterController->Direction = newDirection;
                            filter.CharacterController->IsReady = true;
                        }
                    }
                    else if (filter.CharacterController->StartupLag == filter.CharacterController->AttackOpenLag + 1)
                    {
                        filter.CharacterController->IsReady = true;
                    }
                }

                if (!filter.CharacterController->Invoked)
                {
                    filter.CharacterController->Invoked = true;

                    if (filter.CharacterController->AttackType == AttackType.MainWeapon)
                    {
                        AssetRefMainWeapon mainWeaponAsset = filter.Stats->Build.Equipment.WeaponSettings.MainWeapon;
                        if (f.TryFindAsset(mainWeaponAsset.Id, out MainWeapon mainWeapon))
                        {
                            AnimationRef animRef;

                            if (filter.CharacterController->IsGrounded)
                                animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.Grounded, filter.CharacterController->Direction);
                            else
                                animRef = DirectionalAssetHelper.GetFromDirection(mainWeapon.Aerial, filter.CharacterController->Direction);

                            CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animRef.ID);
                        }
                    }
                    else if (filter.CharacterController->AttackType == AttackType.Skill)
                    {
                        AssetRefSkill skillAsset = DirectionalAssetHelper.GetFromDirection(filter.Stats->Build.Equipment.Skills, filter.CharacterController->Direction);
                        if (f.TryFindAsset(skillAsset.Id, out Skill skill))
                        {
                            CustomAnimator.SetCurrentState(f, filter.CustomAnimator, skill.Move.ID);
                        }
                    }
                    else if (filter.CharacterController->AttackType == AttackType.SubWeapon)
                    {

                    }
                }
            }
            else if (!filter.CharacterController->Invoked)
            {
                filter.CharacterController->Invoked = true;

                if (f.TryFindAsset(filter.Stats->Build.Equipment.WeaponSettings.Ultimate.Id, out Ultimate ultimate))
                {
                    CustomAnimator.SetCurrentState(f, filter.CustomAnimator, ultimate.Move.ID);
                }
            }

            ++filter.CharacterController->StartupLag;
        }

        protected override void Exit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, ref input, settings);

            filter.CharacterController->IsReady = false;
            filter.CharacterController->Invoked = false;
        }
    }
}
