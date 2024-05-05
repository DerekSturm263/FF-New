namespace Quantum
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
        protected abstract int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats);
        protected virtual int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => 0;
        public virtual bool CanInterruptSelf => false;

        private bool DoesStateTypeMatch(ref CharacterControllerSystem.Filter filter)
        {
            StateType state = GetStateType();

            return (state.HasFlag(StateType.Grounded) && filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                || (state.HasFlag(StateType.Aerial) && !filter.CharacterController->GetNearbyCollider(Colliders.Ground));
        }

        public bool TryEnterAndResolveState(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            States[] blacklist = EntranceBlacklist;
            for (int i = 0; i < blacklist.Length; ++i)
            {
                if (filter.CharacterController->IsInState(blacklist[i]))
                    return false;
            }

            if (filter.CharacterController->CanInput && GetInput(ref input) && DoesStateTypeMatch(ref filter) && !filter.CharacterController->IsHolding(GetState()) && CanEnter(f, ref filter, ref input, settings, stats))
            {
                States[] killList = KillStateList;
                for (int i = 0; i < killList.Length; ++i)
                {
                    if (filter.CharacterController->IsInState(killList[i]))
                        CharacterControllerSystem.AllStates[killList[i]].Exit(f, ref filter, ref input, settings, stats);
                }

                Enter(f, ref filter, ref input, settings, stats);
                return true;
            }

            return false;
        }

        public bool TryExitAndResolveState(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            int stateTime = StateTime(f, ref filter, ref input, settings, stats);
            if ((stateTime != -1 && filter.CharacterController->StateTime > stateTime) || CanExit(f, ref filter, ref input, settings, stats))
            {
                Exit(f, ref filter, ref input, settings, stats);
                return true;
            }

            return false;
        }

        protected virtual bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => true;

        protected virtual bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => false;

        protected virtual void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            Log.Debug($"Entered state: {GetType()}");

            CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)GetState(), true);
            filter.CharacterController->SetState(GetState(), true);

            filter.CharacterController->StateTime = 0;
        }

        protected virtual void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            Log.Debug($"Delayed entered state: {GetType()}");
        }

        public virtual void Update(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.CharacterController->StateTime == DelayedEntranceTime(f, ref filter, ref input, settings, stats))
            {
                DelayedEnter(f, ref filter, ref input, settings, stats);
                return;
            }

            Log.Debug($"Update state: {GetType()}");
        }

        protected virtual void Exit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            filter.CharacterController->StateTime = 0;

            filter.CharacterController->SetState(GetState(), false);
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, (int)GetState(), false);

            Log.Debug($"Exited state: {GetType()}");
        }

        public void ForceExit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => Exit(f, ref filter, ref input, settings, stats);
    }
}
