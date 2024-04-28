namespace Quantum.Movement
{
    public unsafe abstract class PlayerState
    {
        [System.Flags]
        public enum StateType
        {
            Grounded = 1 << 0,
            Aerial = 1 << 1
        }

        public abstract States GetState();

        public abstract bool GetInput(ref Input input);
        public abstract StateType GetStateType();
        public virtual States[] EntranceBlacklist => new States[] { };
        public virtual States[] KillStateList => new States[] { };
        protected abstract int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings);
        protected virtual int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => 0;
        public virtual bool CanInterruptSelf => false;

        private bool DoesStateTypeMatch(ref PlayerStateSystem.Filter filter)
        {
            StateType state = GetStateType();

            return (state.HasFlag(StateType.Grounded) && filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                || (state.HasFlag(StateType.Aerial) && !filter.CharacterController->GetNearbyCollider(Colliders.Ground));
        }

        public bool TryEnterAndResolveState(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            States[] blacklist = EntranceBlacklist;
            for (int i = 0; i < blacklist.Length; ++i)
            {
                if (filter.CharacterController->IsInState(blacklist[i]))
                    return false;
            }

            if (GetInput(ref input) && DoesStateTypeMatch(ref filter) && !filter.CharacterController->IsHolding(GetState()) && CanEnter(f, ref filter, ref input, settings))
            {
                States[] killList = KillStateList;
                for (int i = 0; i < killList.Length; ++i)
                {
                    if (filter.CharacterController->IsInState(killList[i]))
                        PlayerStateSystem.AllStates[killList[i]].Exit(f, ref filter, ref input, settings);
                }

                Enter(f, ref filter, ref input, settings);
                return true;
            }

            return false;
        }

        public bool TryExitAndResolveState(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            int stateTime = StateTime(f, ref filter, ref input, settings);
            if ((stateTime != -1 && filter.CharacterController->FramesInState > stateTime) || CanExit(f, ref filter, ref input, settings))
            {
                Exit(f, ref filter, ref input, settings);
                return true;
            }

            return false;
        }

        protected virtual bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => true;

        protected virtual bool CanExit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => false;

        protected virtual void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            Log.Debug($"Entered state: {GetType()}");

            CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)GetState(), true);
            filter.CharacterController->SetState(GetState(), true);

            filter.CharacterController->FramesInState = 0;
        }

        protected virtual void DelayedEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            Log.Debug($"Delayed entered state: {GetType()}");
        }

        public virtual void Update(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            if (filter.CharacterController->FramesInState == DelayedEntranceTime(f, ref filter, ref input, settings))
            {
                DelayedEnter(f, ref filter, ref input, settings);
                return;
            }

            Log.Debug($"Update state: {GetType()}");
        }

        protected virtual void Exit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            filter.CharacterController->FramesInState = 0;

            filter.CharacterController->SetState(GetState(), false);
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)GetState(), false);

            Log.Debug($"Exited state: {GetType()}");
        }

        public void ForceExit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => Exit(f, ref filter, ref input, settings);
    }
}
