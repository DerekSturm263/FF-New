using Quantum.Types;
using System.Collections.Generic;

namespace Quantum
{
    public unsafe class PlayerStateMachine
    {
        private readonly Dictionary<States, PlayerState> _states = [];
        public Dictionary<States, PlayerState> States => _states;

        public PlayerStateMachine(PlayerState[] states)
        {
            foreach (PlayerState state in states)
            {
                _states.Add(state.GetStateInfo().Item1, state);
            }
        }

        public void Resolve(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            // Set some values that any state might check.
            if (!_states[filter.CharacterController->CurrentState].OverrideDirection())
            {
                filter.CharacterController->DirectionValue = input.SnapMovementTo8Directions;
                filter.CharacterController->DirectionEnum = DirectionalHelper.GetEnumFromDirection(input.Movement);
            }

            // Determine the next state to transition to (only if not currently transitioning).
            if (filter.CharacterController->NextState == (Quantum.States)(-1) && _states[filter.CharacterController->CurrentState].TryResolve(f, ref filter, input, settings, this, out TransitionInfo transition))
            {
                // Set the next state and transition time.
                filter.CharacterController->NextState = transition.Destination;
                filter.CharacterController->NextStateTime = filter.CharacterController->StateTime + transition.TransitionTime;

                // Start exiting the current state and entering the new state.
                _states[filter.CharacterController->CurrentState].BeginExit(f, ref filter, input, settings, filter.CharacterController->NextState);
                _states[filter.CharacterController->NextState].BeginEnter(f, ref filter, input, settings, filter.CharacterController->CurrentState);

                Log.Debug($"Transitioning from {filter.CharacterController->CurrentState} to {filter.CharacterController->NextState} for {transition.TransitionTime} frames");
            }
            // If there was no transition...
            else
            {
                // Update current state.
                _states[filter.CharacterController->CurrentState].Update(f, ref filter, input, settings);
            }

            // See if the player needs to transition states.
            if (filter.CharacterController->NextState != (Quantum.States)(-1) && filter.CharacterController->StateTime == filter.CharacterController->NextStateTime)
            {
                Log.Debug($"Transitioning from {filter.CharacterController->CurrentState} to {filter.CharacterController->NextState} now!");

                // Finish exiting the current state.
                _states[filter.CharacterController->CurrentState].FinishExit(f, ref filter, input, settings, filter.CharacterController->NextState);
                States previousState = filter.CharacterController->CurrentState;

                // Set the new state and and reset the state time.
                filter.CharacterController->CurrentState = filter.CharacterController->NextState;
                filter.CharacterController->NextState = (Quantum.States)(-1);
                filter.CharacterController->StateTime = 0;

                // Finish entering the new state.
                _states[filter.CharacterController->CurrentState].FinishEnter(f, ref filter, input, settings, previousState);
            }

            // Increment the state frame.
            ++filter.CharacterController->StateTime;
            filter.CharacterController->LastFrame = input;

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
    }
}
