namespace Quantum
{
    public unsafe partial struct Timer
    {
        public void StartCountdown(int time)
        {
            Time = time;
            IsCounting = true;
        }

        public void PauseCountdown()
        {
            IsCounting = false;
        }

        public void ResumeCountdown()
        {
            IsCounting = true;
        }

        public void StopCountdown()
        {
            PauseCountdown();
        }

        public void SetTime(int time) => Time = time;
    }
}