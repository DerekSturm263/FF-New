using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class Behavior : InfoAsset
    {
        [Tooltip("A higher aggression will cause the AI perform actions more frequently")] public FP Aggression;
        [Tooltip("A low randomness will cause the AI always use the most optimal strategy")] public FP Randomness;
        [Tooltip("A lower sureness will cause the AI to act more carefully and stupidly")] public FP Sureness;
        [Tooltip("A higher teachability will cause the AI to switch things up things that don't work and keep doing things that do work")] public FP Teachability;

        public bool CanMove;
        public bool CanUseAltWeapon;
        public bool CanUseBlock;
        public bool CanUseBurst;
        public bool CanUseCrouch;
        public bool CanUseDodge;
        public bool CanUseEmote;
        public bool CanUseFastFall;
        public bool CanUseInteract;
        public bool CanUseJump;
        public bool CanUseMainWeapon;
        public bool CanUseSub;
        public bool CanUseUltimate;

        public Input GetInput(Frame f, CharacterControllerSystem.Filter userFilter)
        {
            AIData* aiData = f.Unsafe.GetPointer<AIData>(userFilter.Entity);
            CharacterControllerSystem.Filter targetFilter = new()
            {
                Entity = aiData->Target,

                CharacterController = f.Unsafe.GetPointer<CharacterController>(aiData->Target),
                Transform = f.Unsafe.GetPointer<Transform2D>(aiData->Target),
                PhysicsBody = f.Unsafe.GetPointer<PhysicsBody2D>(aiData->Target),
                CustomAnimator = f.Unsafe.GetPointer<CustomAnimator>(aiData->Target),
                Stats = f.Unsafe.GetPointer<Stats>(aiData->Target)
            };

            SetGoals(f, userFilter, aiData, targetFilter);
            return PerformActions(f, userFilter, aiData, targetFilter);
        }

        private void SetGoals(Frame f, CharacterControllerSystem.Filter userFilter, AIData* aiData, CharacterControllerSystem.Filter targetFilter)
        {
            if (targetFilter.Stats->CurrentHealth < 20)
            {
                aiData->SetGoal(Goal.DealHeavyDamage, true);
                aiData->SetGoal(Goal.StartCombo, false);
            }
            else
            {
                aiData->SetGoal(Goal.DealHeavyDamage, false);
                aiData->SetGoal(Goal.StartCombo, true);
            }
        }

        private Input PerformActions(Frame f, CharacterControllerSystem.Filter userFilter, AIData* aiData, CharacterControllerSystem.Filter targetFilter)
        {
            Input input = default;

            if (CanMove)
            {
                FP x = (targetFilter.Transform->Position - userFilter.Transform->Position).Normalized.X;

                if (aiData->CurrentGoal.HasFlag(Goal.IncreaseGap))
                    x *= -1;

                input.Movement = new(x, 0);
            }

            return input;
        }
    }
}
