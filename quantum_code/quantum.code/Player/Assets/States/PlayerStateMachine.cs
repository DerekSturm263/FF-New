using Quantum.Types;

namespace Quantum
{
    public unsafe class PlayerStateMachine
    {
        public void Resolve(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            // Set some values that any state might check.
            if (!f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).OverrideDirection)
            {
                filter.CharacterController->DirectionValue = input.SnapMovementTo8Directions;
                filter.CharacterController->DirectionEnum = DirectionalHelper.GetEnumFromDirection(input.Movement);
            }

            // Determine the next state to transition to (only if not currently transitioning).
            if (!filter.CharacterController->NextState.Id.IsValid && f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).TryResolve(f, this, ref filter, input, settings, out TransitionInfo transition))
            {
                BeginTransition(f, ref filter, input, settings, transition);
            }
            // If there was no transition...
            else
            {
                // Update current state.
                f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).Update(f, this, ref filter, input, settings);
            }

            // See if the player needs to transition states.
            if (filter.CharacterController->NextState.Id.IsValid && filter.CharacterController->StateTime == filter.CharacterController->NextStateTime)
            {
                EndTransition(f, ref filter, input, settings);
            }

            // Increment the state frame.
            ++filter.CharacterController->StateTime;

            // Update some global animation values.
            bool isGrounded = filter.CharacterController->GetNearbyCollider(Colliders.Ground);
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsGrounded", isGrounded);

            if (isGrounded)
                CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YVelocity", 0);
            else
                CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YVelocity", filter.PhysicsBody->Velocity.Y);

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "XDirection", filter.CharacterController->DirectionValue.X);
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YDirection", filter.CharacterController->DirectionValue.Y);
        }

        public void BeginTransition(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, TransitionInfo transition)
        {
            // Set the next state and transition time.
            filter.CharacterController->NextState = transition.Destination;
            filter.CharacterController->NextStateTime = filter.CharacterController->StateTime + transition.TransitionTime;

            // Start exiting the current state and entering the new state.
            f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).BeginExit(f, this, ref filter, input, settings, filter.CharacterController->NextState);
            f.FindAsset<PlayerState>(filter.CharacterController->NextState.Id).BeginEnter(f, this, ref filter, input, settings, filter.CharacterController->CurrentState);

            Log.Debug($"Transitioning from {filter.CharacterController->CurrentState} to {filter.CharacterController->NextState} for {transition.TransitionTime} frames");
        }

        public void ForceTransition(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState state, int transitionTime)
        {
            // Set the next state and transition time.
            filter.CharacterController->NextState = state;
            filter.CharacterController->NextStateTime = filter.CharacterController->StateTime + transitionTime;

            // Start exiting the current state and entering the new state.
            f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).BeginExit(f, this, ref filter, input, settings, filter.CharacterController->NextState);
            f.FindAsset<PlayerState>(filter.CharacterController->NextState.Id).BeginEnter(f, this, ref filter, input, settings, filter.CharacterController->CurrentState);

            Log.Debug($"Transitioning from {filter.CharacterController->CurrentState} to {filter.CharacterController->NextState} for {transitionTime} frames");
        }

        public void EndTransition(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug($"Transitioning from {filter.CharacterController->CurrentState} to {filter.CharacterController->NextState} now!");

            // Finish exiting the current state.
            f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).FinishExit(f, this, ref filter, input, settings, filter.CharacterController->NextState);
            AssetRefPlayerState previousState = filter.CharacterController->CurrentState;

            // Set the new state and and reset the state time.
            filter.CharacterController->CurrentState = filter.CharacterController->NextState;
            filter.CharacterController->NextState.Id = 0;
            filter.CharacterController->StateTime = 0;

            // Finish entering the new state.
            f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).FinishEnter(f, this, ref filter, input, settings, previousState);
        }
    }
}
