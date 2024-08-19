namespace Quantum
{
    public struct TransitionInfo(States destination, int transitionTime, bool overrideExit, bool overrideEnter)
    {
        public States Destination = destination;
        public int TransitionTime = transitionTime;

        public bool OverrideExit = overrideExit;
        public bool OverrideEnter = overrideEnter;
    }

    public unsafe abstract class PlayerState
    {
        [System.Flags]
        public enum EntranceType
        {
            Grounded = 1 << 0,
            Aerial = 1 << 1
        }

        public enum AnimationType
        {
            Hold,
            Trigger
        }

        protected abstract bool IsInputting(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, ref Input input);

        public abstract (States, StatesFlag) GetStateInfo();
        public abstract EntranceType GetEntranceType();
        public abstract AnimationType GetAnimationType();

        public virtual bool OverrideDirection() => false;

        public abstract TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);

        public bool TryResolve(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, out TransitionInfo outTransition)
        {
            bool canExit = CanExit(f, stateMachine, ref filter, input, settings);

            foreach (var transition in GetTransitions(f, stateMachine, ref filter, input, settings))
            {
                if (!transition.OverrideExit && !canExit)
                    continue;

                if (transition.OverrideEnter || stateMachine.States[transition.Destination].CanEnter(f, stateMachine, ref filter, input, settings))
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
            var stateInfo = GetStateInfo();

            return filter.CharacterController->CanInput &&
                   filter.CharacterController->PossibleStates.HasFlag(stateInfo.Item2) &&
                   IsInputting(stateMachine, ref filter, ref input) &&
                   DoesStateTypeMatch(stateMachine, ref filter);
        }

        public virtual void BeginEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            Log.Debug($"Beginning entering state: {GetType()}");
        }

        public virtual void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            Log.Debug($"Finishing entering state: {GetType()}");

            InitializeAnimator(f, filter.CustomAnimator);
        }

        public virtual void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug($"Update state: {GetType()}");
        }

        protected abstract bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);

        public virtual void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            ShutdownAnimator(f, filter.CustomAnimator);

            Log.Debug($"Beginning exiting state: {GetType()}");
        }

        public virtual void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            Log.Debug($"Finishing exiting state: {GetType()}");
        }

        private bool DoesStateTypeMatch(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter)
        {
            EntranceType state = GetEntranceType();

            return (state.HasFlag(EntranceType.Grounded) && filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                || (state.HasFlag(EntranceType.Aerial) && !filter.CharacterController->GetNearbyCollider(Colliders.Ground));
        }

        private void InitializeAnimator(Frame f, CustomAnimator* customAnimator)
        {
            CustomAnimator.SetBoolean(f, customAnimator, (int)GetStateInfo().Item1, true);
        }
        
        private void ShutdownAnimator(Frame f, CustomAnimator* customAnimator)
        {
            CustomAnimator.SetBoolean(f, customAnimator, (int)GetStateInfo().Item1, false);
        }
    }

    public unsafe abstract class PassiveState : PlayerState
    {
        public sealed override AnimationType GetAnimationType() => AnimationType.Hold;

        protected abstract bool DoExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);
        protected sealed override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => DoExit(f, stateMachine, ref filter, input, settings);
    }

    public unsafe abstract class ExclusivePassiveState : PassiveState
    {
        protected abstract Input.Buttons GetInput();
        protected override bool IsInputting(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, ref Input input) => filter.CharacterController->WasPressedThisFrame(input, GetInput());

        protected override sealed bool DoExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => !filter.CharacterController->IsHeldThisFrame(input, GetInput());
    }

    public unsafe abstract class ActionState : PlayerState
    {
        protected abstract Input.Buttons GetInput();
        protected override bool IsInputting(PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, ref Input input) => filter.CharacterController->WasPressedThisFrame(input, GetInput());

        public sealed override AnimationType GetAnimationType() => AnimationType.Trigger;

        protected abstract int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);
        protected override sealed bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->StateTime >= StateTime(f, stateMachine, ref filter, input, settings);
    }
}
