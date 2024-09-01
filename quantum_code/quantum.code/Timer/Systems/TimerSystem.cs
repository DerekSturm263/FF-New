using Photon.Deterministic;
using System;

namespace Quantum
{
    public unsafe class TimerSystem : SystemMainThreadFilter<TimerSystem.Filter>, ISignalOnComponentAdded<Timer>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Timer* Timer;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (!filter.Timer->IsCounting || f.DeltaTime == FP._0)
                return;

            CountDown(f, filter.Timer);

            if (filter.Timer->Time > filter.Timer->Start)
                UpdatePlayerStats(f, filter.Timer);
        }

        private void CountDown(Frame f, Timer* timer)
        {
            timer->Time += timer->Step;

            if (timer->Time % 60 == 0)
                CountDownOneSecond(f, timer, timer->Time, timer->Time / 60, true);
        }

        private static void CountDownOneSecond(Frame f, Timer* timer, int ticks, int seconds, bool invokeEvent)
        {
            f.Events.OnTimerTick(seconds, invokeEvent);

            if (ticks >= timer->Start)
            {
                f.Events.OnBeginningCountdown(seconds - (timer->Start / 60), invokeEvent);
            }
            
            if (ticks == timer->Start)
            {
                MatchSystem.StartOfMatch(f, FighterIndex.GetAllTeams(f));
            }
            else if (ticks == timer->End)
            {
                timer->IsCounting = false;

                f.Global->IsTimerOver = true;
            }

            if (seconds == 61)
            {
                f.Events.OnOneMinuteLeft();
            }
        }

        private void UpdatePlayerStats(Frame f, Timer* timer)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                FP lerpValue = (FP)(timer->Time - timer->Start) / 180;

                FP health = FPMath.Lerp(f.Global->CurrentMatch.Ruleset.Players.MaxHealth, 0, lerpValue);
                StatsSystem.SetHealth(f, stats.Entity, stats.Component, health, false);

                FP energy = FPMath.Lerp(f.Global->CurrentMatch.Ruleset.Players.MaxEnergy / 5, 0, lerpValue);
                StatsSystem.SetEnergy(f, stats.Entity, stats.Component, energy);

                int stocks = FPMath.Lerp(f.Global->CurrentMatch.Ruleset.Players.StockCount, 0, lerpValue).AsInt;
                StatsSystem.SetStocks(f, stats.Entity, stats.Component, stocks);
            }
        }

        public void OnAdded(Frame f, EntityRef entity, Timer* component)
        {
            SetTime(f, new(0, 0, f.Global->CurrentMatch.Ruleset.Match.Time), true);
        }

        public static void StartCountdown(Frame f, TimeSpan time)
        {
            SetTime(f, time, true);
            ResumeCountdown(f);
        }

        public static void PauseCountdown(Frame f)
        {
            f.Unsafe.GetPointerSingleton<Timer>()->IsCounting = false;
        }

        public static void ResumeCountdown(Frame f)
        {
            f.Unsafe.GetPointerSingleton<Timer>()->IsCounting = true;
        }

        public static void StopCountdown(Frame f)
        {
            PauseCountdown(f);

            if (f.Unsafe.TryGetPointerSingleton(out Timer* timer))
            {
                timer->Time = timer->OriginalTime;
                timer->Start = timer->OriginalTime - 180;
            }
        }

        public static void SetTime(Frame f, TimeSpan time, bool invokeEvent)
        {
            if (f.Unsafe.TryGetPointerSingleton(out Timer* timer))
            {
                timer->OriginalTime = Convert.ToInt32(time.TotalSeconds) * 60;

                timer->Time = timer->OriginalTime;
                timer->Start = timer->OriginalTime - 180;

                CountDownOneSecond(f, timer, timer->Time, timer->Time / 60, invokeEvent);
            }
        }
    }
}
