namespace Quantum
{
    public unsafe partial struct AIData
    {
        public void SetGoal(Goal goal, bool setGoal)
        {
            if (setGoal == true)
                CurrentGoal |= goal;
            else
                CurrentGoal &= ~goal;
        }

        public readonly bool GetGoal(Goal goal) => CurrentGoal.HasFlag(goal);
    }
}
