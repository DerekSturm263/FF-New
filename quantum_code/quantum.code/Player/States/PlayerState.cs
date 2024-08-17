namespace Quantum
{
    public struct TransitionInfo
    {
        public States Destination;
        public bool OverrideDefaultTransitionConditions;
        public System.Func<Frame, CharacterControllerSystem.Filter, Input, MovementSettings, bool> CanTransition;

        public TransitionInfo()
        {
            OverrideDefaultTransitionConditions = false;
            CanTransition = (_, _, _, _) => true;
        }

        public TransitionInfo(bool overrideDefaultTransitionConditions)
        {
            OverrideDefaultTransitionConditions = overrideDefaultTransitionConditions;
            CanTransition = (_, _, _, _) => true;
        }

        public TransitionInfo(bool overrideDefaultTransitionConditions, System.Func<Frame, CharacterControllerSystem.Filter, Input, MovementSettings, bool> canTransition)
        {
            OverrideDefaultTransitionConditions = overrideDefaultTransitionConditions;
            CanTransition = canTransition;
        }
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

        protected abstract bool IsInputting(ref CharacterControllerSystem.Filter filter, ref Input input);

        public abstract (States, StatesFlag) GetStateInfo();
        public abstract EntranceType GetEntranceType();
        public abstract AnimationType GetAnimationType();

        public abstract TransitionInfo[] GetTransitions();

        public bool TryResolve(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, PlayerStateMachine stateMachine, out States newState)
        {
            bool canExit = CanExit(f, ref filter, input, settings);

            foreach (var transition in GetTransitions())
            {
                if (!transition.OverrideDefaultTransitionConditions && !canExit)
                    continue;

                if (stateMachine.States[transition.Destination].CanEnter(f, ref filter, input, settings) && transition.CanTransition.Invoke(f, filter, input, settings))
                {
                    newState = transition.Destination;
                    return true;
                }
            }

            newState = GetStateInfo().Item1;
            return false;
        }

        public virtual int GetEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => 0;

        protected virtual bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            var stateInfo = GetStateInfo();

            return filter.CharacterController->CanInput &&
                   filter.CharacterController->PossibleStates.HasFlag(stateInfo.Item2) &&
                   IsInputting(ref filter, ref input) &&
                   DoesStateTypeMatch(ref filter);
        }

        public virtual void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug($"Entered state: {GetType()}");

            InitializeAnimator(f, filter.CustomAnimator);
        }

        public virtual void Update(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            Log.Debug($"Update state: {GetType()}");
        }

        protected abstract bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);

        public virtual void Exit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            ShutdownAnimator(f, filter.CustomAnimator);

            Log.Debug($"Exited state: {GetType()}");
        }

        private bool DoesStateTypeMatch(ref CharacterControllerSystem.Filter filter)
        {
            EntranceType state = GetEntranceType();

            return (state.HasFlag(EntranceType.Grounded) && filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                || (state.HasFlag(EntranceType.Aerial) && !filter.CharacterController->GetNearbyCollider(Colliders.Ground));
        }

        private void InitializeAnimator(Frame f, CustomAnimator* customAnimator)
        {
            if (GetAnimationType() == AnimationType.Hold)
                CustomAnimator.SetBoolean(f, customAnimator, (int)GetStateInfo().Item1, true);
            else
                CustomAnimator.SetTrigger(f, customAnimator, (int)GetStateInfo().Item1);
        }
        
        private void ShutdownAnimator(Frame f, CustomAnimator* customAnimator)
        {
            if (GetAnimationType() == AnimationType.Hold)
                CustomAnimator.SetBoolean(f, customAnimator, (int)GetStateInfo().Item1, false);
        }
    }

    public unsafe abstract class PassiveState : PlayerState
    {
        public sealed override AnimationType GetAnimationType() => AnimationType.Hold;

        protected abstract bool DoExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);
        protected sealed override bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => DoExit(f, ref filter, input, settings);
    }

    public unsafe abstract class ExclusivePassiveState : PassiveState
    {
        protected abstract Input.Buttons GetInput();
        protected override bool IsInputting(ref CharacterControllerSystem.Filter filter, ref Input input) => filter.CharacterController->IsHeldThisFrame(input, GetInput());

        protected override sealed bool DoExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => !IsInputting(ref filter, ref input);
    }

    public unsafe abstract class ActionState : PlayerState
    {
        protected abstract Input.Buttons GetInput();
        protected override bool IsInputting(ref CharacterControllerSystem.Filter filter, ref Input input) => filter.CharacterController->WasPressedThisFrame(input, GetInput());

        public sealed override AnimationType GetAnimationType() => AnimationType.Trigger;

        protected abstract int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings);
        protected override sealed bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->StateTime >= StateTime(f, ref filter, input, settings);
    }

    public unsafe abstract class DirectionalActionState : ActionState
    {
        public sealed override int GetEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => settings.DirectionChangeTime;
    }
}
