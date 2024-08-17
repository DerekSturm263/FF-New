using Quantum.Types;
using System.Collections.Generic;
using static Quantum.PlayerState;

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
            filter.CharacterController->DirectionValue = input.SnapMovementTo8Directions;
            filter.CharacterController->DirectionEnum = DirectionalHelper.GetEnumFromDirection(input.Movement);

            // Determine the next state to transition to (only if not currently transitioning).
            if (filter.CharacterController->NextState == (Quantum.States)(-1) && _states[filter.CharacterController->CurrentState].TryResolve(f, ref filter, input, settings, this, out States newState))
            {
                // Set the next state.
                filter.CharacterController->NextState = newState;

                // Get how long to take to transition.
                int entranceTime = _states[filter.CharacterController->NextState].GetEntranceTime(f, ref filter, input, settings);
                filter.CharacterController->NextStateTime = filter.CharacterController->StateTime + entranceTime;

                Log.Debug($"Transitioning from {filter.CharacterController->CurrentState} to {filter.CharacterController->NextState} in {entranceTime} frames");
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

                // Exit the current state.
                _states[filter.CharacterController->CurrentState].Exit(f, ref filter, input, settings);

                // Set the new state and and reset the state time.
                filter.CharacterController->CurrentState = filter.CharacterController->NextState;
                filter.CharacterController->NextState = (Quantum.States)(-1);
                filter.CharacterController->StateTime = 0;

                // Enter the new state.
                _states[filter.CharacterController->CurrentState].Enter(f, ref filter, input, settings);
            }

            // Increment the state frame.
            ++filter.CharacterController->StateTime;
            filter.CharacterController->LastFrame = input;
        }
    }
}
