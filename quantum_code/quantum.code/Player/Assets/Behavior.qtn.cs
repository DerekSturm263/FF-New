using Photon.Deterministic;
using Quantum.Inspector;
using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class Behavior : InfoAsset
    {
        public List<AssetRefPlayerState> States;

        [Tooltip("Whether or not this Behavior is controllable by a human")] public bool IsControllable;
        [Tooltip("A higher value will cause the AI perform actions more frequently")] public FP ActionSwitchTime;
        [Tooltip("A higher value will cause the AI to randomize its inputs to be more human")] public FP Randomness;
        [Tooltip("A higher value will cause the AI to switch things up things that don't work and keep doing things that do work")] public FP Teachability;
        [Tooltip("A higher value will cause the AI to change its target more often")] public FP TargetSwitchTime;

        public Input GetInput(Frame f, CharacterControllerSystem.Filter userFilter)
        {
            AIData* aiData = f.Unsafe.GetPointer<AIData>(userFilter.Entity);

            SetTarget(f, userFilter, aiData);
            if (!aiData->Target.IsValid)
                return default;

            CharacterControllerSystem.Filter targetFilter = new()
            {
                Entity = aiData->Target,

                CharacterController = f.Unsafe.GetPointer<CharacterController>(aiData->Target),
                Transform = f.Unsafe.GetPointer<Transform2D>(aiData->Target),
                PhysicsBody = f.Unsafe.GetPointer<PhysicsBody2D>(aiData->Target),
                CustomAnimator = f.Unsafe.GetPointer<CustomAnimator>(aiData->Target),
                Stats = f.Unsafe.GetPointer<Stats>(aiData->Target),
                PlayerStats = f.Unsafe.GetPointer<PlayerStats>(aiData->Target),
                Shakeable = f.Unsafe.GetPointer<Shakeable>(aiData->Target)
            };

            SetGoals(f, userFilter, aiData, targetFilter);
            return PerformActions(f, userFilter, aiData, targetFilter);
        }

        private void SetTarget(Frame f, CharacterControllerSystem.Filter userFilter, AIData* aiData)
        {
            if (aiData->TimeSinceTargetSwitch >= TargetSwitchTime)
            {
                FP leastHealth = 100000;

                var playerFilter = f.Unsafe.FilterStruct<CharacterControllerSystem.Filter>();
                var player = default(CharacterControllerSystem.Filter);

                while (playerFilter.Next(&player))
                {
                    if (player.PlayerStats->Index.Team == userFilter.PlayerStats->Index.Team)
                        continue;

                    if (player.Stats->CurrentStats.Health < leastHealth)
                    {
                        leastHealth = player.Stats->CurrentStats.Health;
                        aiData->Target = player.Entity;
                    }
                }

                aiData->TimeSinceTargetSwitch = 0;
            }

            aiData->TimeSinceTargetSwitch += f.DeltaTime;
        }

        private void SetGoals(Frame f, CharacterControllerSystem.Filter userFilter, AIData* aiData, CharacterControllerSystem.Filter targetFilter)
        {
            if (targetFilter.Stats->CurrentStats.Health < 20)
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

            FP x = (targetFilter.Transform->Position - userFilter.Transform->Position).Normalized.X;
            if (aiData->CurrentGoal.HasFlag(Goal.IncreaseGap))
                x *= -1;

            input.Movement = new(x, 0);

            return input;
        }
    }
}
