namespace Quantum.Movement
{
    public unsafe abstract class MovementState
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

        private bool DoesStateTypeMatch(ref MovementSystem.Filter filter)
        {
            StateType state = GetStateType();

            return (state.HasFlag(StateType.Grounded) && filter.CharacterController->IsGrounded)
                || (state.HasFlag(StateType.Aerial) && !filter.CharacterController->IsGrounded);
        }

        public bool TryEnterAndResolveState(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            States[] blacklist = EntranceBlacklist;
            for (int i = 0; i < blacklist.Length; ++i)
            {
                if (filter.CharacterController->IsInState(blacklist[i]))
                    return false;
            }

            if (GetInput(ref input) && DoesStateTypeMatch(ref filter) && CanEnter(f, ref filter, ref input, settings))
            {
                States[] killList = KillStateList;
                for (int i = 0; i < killList.Length; ++i)
                {
                    if (filter.CharacterController->IsInState(killList[i]))
                        MovementSystem.AllStates[killList[i]].Exit(f, ref filter, ref input, settings);
                }

                Enter(f, ref filter, ref input, settings);
                return true;
            }

            return false;
        }

        public bool TryExitAndResolveState(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            if (CanExit(f, ref filter, ref input, settings))
            {
                Exit(f, ref filter, ref input, settings);
                return true;
            }

            return false;
        }

        protected virtual bool CanEnter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings) => true;

        protected abstract bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings);

        protected virtual void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            Log.Debug($"Entered state: {GetType()}");

            CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)GetState(), true);
            filter.CharacterController->SetState(GetState(), true);
        }

        public virtual void Update(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            Log.Debug($"Update state: {GetType()}");
        }

        protected virtual void Exit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            filter.CharacterController->SetState(GetState(), false);
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)GetState(), false);

            Log.Debug($"Exited state: {GetType()}");
        }

        public void ForceExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings) => Exit(f, ref filter, ref input, settings);
    }
}
