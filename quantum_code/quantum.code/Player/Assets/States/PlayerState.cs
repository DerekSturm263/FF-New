using Photon.Deterministic;
using Quantum.Inspector;
using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public struct InputInfo()
    {
        [System.Flags]
        public enum When
        {
            OnPress = 1 << 0,
            OnHold = 1 << 1,
            OnRelease = 1 << 2
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

    [System.Serializable]
    public struct StateKeepingInfo
    {
        public NullableFP XVelocity;
        public NullableFP GravityScale;
        public NullableFP AnimatorSpeed;
    }

    [System.Serializable]
    public struct MovementSettingsGroup
    {
        public MovementMoveSettings GroundedMovement;
        public MovementMoveSettings AerialMovement;

        public FP MovementInfluence;
    }

    [System.Serializable]
    public struct JumpSettingsGroup
    {
        public MovementCurveSettings ShortJump;
        public MovementCurveSettings FullJump;
        public MovementCurveSettings AerialJump;
        public FP MinimumYVelocity;
        public FP FastFallForce;

        public bool CanJump;
    }

    public unsafe partial class PlayerState
    {
        [System.Flags]
        public enum EntranceType
        {
            Grounded = 1 << 0,
            Aerial = 1 << 1
        }

        [Header("Info")]

        public States StateType;
        public EntranceType EntranceAvailability;

        [Header("State Keeping")]

        public StateKeepingInfo OnEnter;
        public StateKeepingInfo OnExit;
        public bool CanCollide = true;

        [Header("Movement")]
        public MovementSettingsGroup Movement;
        public JumpSettingsGroup Jump;

        [Header("Other")]

        public bool OverrideDirection;
        public List<TransitionInfo> Transitions;
        public WeaponState WeaponState;

        public bool TryResolve(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, out TransitionInfo outTransition)
        {
            bool canExit = CanExit(f, stateMachine, ref filter, input, settings);

            foreach (var transition in Transitions)
            {
                if (!transition.InterruptCurrentState && !canExit)
                    continue;

                if (transition.RequiresInput &&
                    (transition.InputInfo.Condition.HasFlag(InputInfo.When.OnPress) && !filter.CharacterController->WasPressedThisFrame(input, transition.InputInfo.Buttons) ^ transition.InputInfo.DoInvert) ||
                    (transition.InputInfo.Condition.HasFlag(InputInfo.When.OnHold) && !filter.CharacterController->IsHeldThisFrame(input, transition.InputInfo.Buttons) ^ transition.InputInfo.DoInvert) ||
                    (transition.InputInfo.Condition.HasFlag(InputInfo.When.OnRelease) && !filter.CharacterController->WasReleasedThisFrame(input, transition.InputInfo.Buttons) ^ transition.InputInfo.DoInvert))
                        continue;

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

            f.Events.OnSwitchWeapon(filter.Entity, filter.PlayerStats->Index, WeaponState);
        }

        public virtual void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            Log.Debug($"Finishing entering state: {GetType()}");

            if (OnEnter.XVelocity.HasValue)
                filter.CharacterController->Velocity = OnEnter.XVelocity.Value;

            if (OnEnter.GravityScale.HasValue)
                filter.PhysicsBody->GravityScale = OnEnter.GravityScale.Value;

            if (OnEnter.AnimatorSpeed.HasValue)
                filter.CustomAnimator->speed = OnEnter.AnimatorSpeed.Value;

            if (!CanCollide)
                filter.PhysicsCollider->Layer = settings.NoPlayerCollision;

            InitializeAnimator(f, filter.CustomAnimator);
        }

        public virtual void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug($"Update state: {GetType()}");

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            HandleMovement(f, stateMachine, ref filter, input, settings, stats);
            HandleJumping(f, stateMachine, ref filter, input, settings, stats);
            HandleFastFalling(f, stateMachine, ref filter, input, settings);
        }

        protected virtual FP GetMovementInfluence(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => Movement.MovementInfluence;

        private void HandleMovement(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            filter.CharacterController->Move(f, input.Movement.X, ref filter, settings, this, stats, GetMovementInfluence(f, stateMachine, ref filter, input, settings));
        }

        private void HandleJumping(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            if ((filter.CharacterController->CanJump || Jump.CanJump) && filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Jump) && filter.CharacterController->JumpCount > 0)
            {
                filter.CharacterController->JumpBuffer = 4;
            }

            if (filter.CharacterController->JumpBuffer > 0)
            {
                --filter.CharacterController->JumpBuffer;

                if (filter.CharacterController->JumpBuffer == 0)
                {
                    filter.CharacterController->GroundedJump = filter.CharacterController->GetNearbyCollider(Colliders.Ground);

                    if (filter.CharacterController->GroundedJump)
                    {
                        filter.CharacterController->NearbyColliders &= ~Colliders.Ground;

                        if (!filter.CharacterController->IsHeldThisFrame(input, Input.Buttons.Jump) || filter.CharacterController->IsHeldThisFrame(input, Input.Buttons.Crouch))
                            filter.CharacterController->JumpType = JumpType.ShortHop;
                        else
                            filter.CharacterController->JumpType = JumpType.FullHop;
                    }
                    else
                    {
                        filter.CharacterController->JumpType = JumpType.Aerial;
                    }

                    --filter.CharacterController->JumpCount;
                    filter.CharacterController->JumpTime = 0;

                    f.Events.OnPlayerJump(filter.Entity, filter.PlayerStats->Index, stats.Jump.AsInt - filter.CharacterController->JumpCount);
                }
            }

            if (filter.CharacterController->JumpType != JumpType.None)
            {
                ++filter.CharacterController->JumpTime;

                MovementCurveSettings jumpSettings = filter.CharacterController->GetJumpSettings(this);
                filter.PhysicsBody->Velocity.Y = jumpSettings.Curve.Evaluate(filter.CharacterController->JumpTime) * (jumpSettings.Force * (1 / stats.Weight));

                if (filter.CharacterController->JumpTime >= filter.CharacterController->GetJumpSettings(this).Frames)
                {
                    filter.CharacterController->JumpType = JumpType.None;
                    filter.CharacterController->JumpTime = 0;
                }

                CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)States.Jump, true);
            }
            else
            {
                CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)States.Jump, false);
            }
        }

        private void HandleFastFalling(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Crouch) && filter.PhysicsBody->Velocity.Y < Jump.MinimumYVelocity)
            {
                filter.PhysicsBody->Velocity.Y = Jump.FastFallForce;
            }
        }

        protected virtual bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => true;

        public virtual void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            ShutdownAnimator(f, filter.CustomAnimator);

            f.Events.OnSwitchWeapon(filter.Entity, filter.PlayerStats->Index, WeaponState.Default);

            Log.Debug($"Beginning exiting state: {GetType()}");
        }

        public virtual void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            if (OnExit.XVelocity.HasValue)
                filter.CharacterController->Velocity = OnExit.XVelocity.Value;

            if (OnExit.GravityScale.HasValue)
                filter.PhysicsBody->GravityScale = OnExit.GravityScale.Value;

            if (OnExit.AnimatorSpeed.HasValue)
                filter.CustomAnimator->speed = OnExit.AnimatorSpeed.Value;

            if (!CanCollide)
                filter.PhysicsCollider->Layer = settings.PlayerCollision;

            Log.Debug($"Finishing exiting state: {GetType()}");
        }

        protected bool DoesStateTypeMatch(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter)
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
        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->StateTime >= StateTime(f, stateMachine, ref filter, input, settings);
    }

    [System.Serializable]
    public unsafe class InputState : PlayerState
    {
        [Header("State-Specific Values")]

        public Input.Buttons Button;

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.CharacterController->WasReleasedThisFrame(input, Button);
        }
    }
}
