using Photon.Deterministic;
using System;
using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public struct InputInfo()
    {
        public enum When
        {
            OnPress,
            OnHold,
            OnRelease
        }

        public Input.Buttons Buttons;
        public When Condition;
        public bool DoInvert;
    }

    [System.Serializable]
    public struct TransitionInfo
    {
        public AssetRefPlayerState Destination;
        public int TransitionTime;

        public bool RequiresInput;
        public InputInfo InputInfo;

        public bool InterruptCurrentState;
    }

    [Serializable]
    public struct StatekeepingInfo
    {
        public bool ManipulateVelocity;
        public FP Velocity;

        public bool ManipulateGravity;
        public FP Gravity;
    }

    public unsafe abstract partial class PlayerState
    {
        [System.Flags]
        public enum EntranceType
        {
            Grounded = 1 << 0,
            Aerial = 1 << 1
        }

        public States StateType;

        public EntranceType EntranceAvailability;

        public List<TransitionInfo> Transitions;

        public bool OverrideDirection;
        public Colliders MutuallyExclusiveColliders;

        public MovementMoveSettings GroundedMovement;
        public MovementMoveSettings AerialMovement;

        public FP MinimumYVelocity;
        public FP FastFallForce;

        public StatekeepingInfo OnEnter;
        public StatekeepingInfo OnExit;

        public FP MovementInfluence;

        public bool DisableCollision;

        public bool TryResolve(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, out TransitionInfo outTransition)
        {
            bool canExit = CanExit(f, stateMachine, ref filter, input, settings);

            foreach (var transition in Transitions)
            {
                if (!transition.InterruptCurrentState && !canExit)
                    continue;

                if (transition.RequiresInput)
                {
                    switch (transition.InputInfo.Condition)
                    {
                        case InputInfo.When.OnPress:
                            if (!filter.CharacterController->WasPressedThisFrame(input, transition.InputInfo.Buttons) ^ transition.InputInfo.DoInvert)
                                continue;

                            break;

                        case InputInfo.When.OnHold:
                            if (!filter.CharacterController->IsHeldThisFrame(input, transition.InputInfo.Buttons) ^ transition.InputInfo.DoInvert)
                                continue;

                            break;

                        case InputInfo.When.OnRelease:
                            if (!filter.CharacterController->WasReleasedThisFrame(input, transition.InputInfo.Buttons) ^ transition.InputInfo.DoInvert)
                                continue;

                            break;
                    }
                }

                if (f.FindAsset<PlayerState>(transition.Destination.Id).CanEnter(f, stateMachine, ref filter, input, settings))
                {
                    outTransition = transition;
                    return true;
                }
            }

            outTransition = default;
            return false;
        }

        protected virtual bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.CharacterController->CanInput &&
                   filter.CharacterController->PossibleStates.HasFlag((StatesFlag)System.Math.Pow(2, (int)StateType)) &&
                   filter.Shakeable->Time <= 0 &&
                   DoesStateTypeMatch(stateMachine, ref filter);
        }

        public virtual void BeginEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            Log.Debug($"Beginning entering state: {GetType()}");
        }

        public virtual void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            Log.Debug($"Finishing entering state: {GetType()}");

            if (OnEnter.ManipulateVelocity)
                filter.CharacterController->Velocity = OnEnter.Velocity;

            if (OnEnter.ManipulateGravity)
                filter.PhysicsBody->GravityScale = OnEnter.Gravity;

            if (DisableCollision)
                filter.PhysicsCollider->Layer = settings.NoPlayerCollision;

            InitializeAnimator(f, filter.CustomAnimator);
        }

        public virtual void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug($"Update state: {GetType()}");

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            HandleMovement(f, stateMachine, ref filter, input, settings, stats);
            HandleFastFalling(f, stateMachine, ref filter, input, settings);
        }

        protected virtual FP GetMovementInfluence(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => MovementInfluence;

        private void HandleMovement(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            filter.CharacterController->Move(f, input.Movement.X, ref filter, settings, this, stats, GetMovementInfluence(f, stateMachine, ref filter, input, settings));
        }

        private void HandleFastFalling(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Crouch) && filter.PhysicsBody->Velocity.Y < MinimumYVelocity)
            {
                filter.PhysicsBody->Velocity.Y = FastFallForce;
            }
        }

        protected abstract bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);

        public virtual void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            ShutdownAnimator(f, filter.CustomAnimator);

            Log.Debug($"Beginning exiting state: {GetType()}");
        }

        public virtual void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            if (OnExit.ManipulateVelocity)
                filter.CharacterController->Velocity = OnExit.Velocity;

            if (OnExit.ManipulateGravity)
                filter.PhysicsBody->GravityScale = OnExit.Gravity;

            if (DisableCollision)
                filter.PhysicsCollider->Layer = settings.PlayerCollision;

            Log.Debug($"Finishing exiting state: {GetType()}");
        }

        private bool DoesStateTypeMatch(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter)
        {
            return (EntranceAvailability.HasFlag(EntranceType.Grounded) && filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                || (EntranceAvailability.HasFlag(EntranceType.Aerial) && !filter.CharacterController->GetNearbyCollider(Colliders.Ground));
        }

        private void InitializeAnimator(Frame f, CustomAnimator* customAnimator)
        {
            CustomAnimator.SetBoolean(f, customAnimator, (int)StateType, true);
        }
        
        private void ShutdownAnimator(Frame f, CustomAnimator* customAnimator)
        {
            CustomAnimator.SetBoolean(f, customAnimator, (int)StateType, false);
        }
    }

    [System.Serializable]
    public unsafe abstract class ActionState : PlayerState
    {
        protected abstract int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);
        protected override sealed bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->StateTime >= StateTime(f, stateMachine, ref filter, input, settings);
    }
}
