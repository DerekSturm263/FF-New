using Photon.Deterministic;
using System;
using System.ComponentModel;

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
            if (!filter.Timer->IsCounting)
                return;

            CountDown(f, filter.Timer);

            if (filter.Timer->Time > filter.Timer->Start)
                UpdatePlayerStats(f, filter.Timer);
        }

        private void CountDown(Frame f, Timer* timer)
        {
            timer->Time += timer->Step;

            if (timer->Time % 60 == 0)
                CountDownOneSecond(f, timer, timer->Time, timer->Time / 60);
        }

        private static void CountDownOneSecond(Frame f, Timer* timer, int ticks, int seconds)
        {
            f.Events.OnTimerTick(seconds);

            if (ticks >= timer->Start)
            {
                f.Events.OnBeginningCountdown(seconds - (timer->Start / 60));
            }
            
            if (ticks == timer->Start)
            {
                MatchSystem.StartOfMatch(f);
            }
            else if (ticks == timer->End)
            {
                timer->IsCounting = false;

                if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
                    matchInstance->IsTimerOver = true;
            }

            if (seconds == 61)
            {
                f.Events.OnOneMinuteLeft();
            }
        }

        private void UpdatePlayerStats(Frame f, Timer* timer)
        {
            foreach (var playerLink in f.Unsafe.GetComponentBlockIterator<PlayerLink>())
            {
                if (f.Unsafe.TryGetPointer(playerLink.Entity, out Stats* stats))
                {
                    FP lerpValue = (timer->Start - timer->Time) / timer->OriginalTime;

                    FP health = FPMath.Lerp(stats->MaxHealth, 0, lerpValue);
                    StatsSystem.SetHealth(f, playerLink.Component, stats, health);

                    FP energy = FPMath.Lerp(stats->MaxEnergy / 5, 0, lerpValue);
                    StatsSystem.SetEnergy(f, playerLink.Component, stats, energy);

                    int stocks = FPMath.Lerp(stats->MaxStocks, 0, lerpValue).AsInt;
                    StatsSystem.SetStocks(f, playerLink.Component, stats, stocks);
                }
            }
        }

        public void OnAdded(Frame f, EntityRef entity, Timer* component)
        {
            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
                SetTime(f, new(0, 0, matchInstance->Match.Ruleset.Match.Time));

            CountDownOneSecond(f, component, component->Time, component->Time / 60);
        }

        public static void StartCountdown(Frame f, TimeSpan time)
        {
            SetTime(f, time);
            ResumeCountdown(f);
        }

        public static void PauseCountdown(Frame f)
        {
            f.Unsafe.GetPointerSingleton<Timer>()->IsCounting = false;
        }

        public static void ResumeCountdown(Frame f)
        {
            f.Unsafe.GetPointerSingleton<Timer>()->IsCounting = true;

            if (f.Unsafe.TryGetPointerSingleton(out Timer* timer))
            {
                CountDownOneSecond(f, timer, timer->Time, timer->Time / 60);
            }
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

        public static void SetTime(Frame f, TimeSpan time)
        {
            if (f.Unsafe.TryGetPointerSingleton(out Timer* timer))
            {
                timer->OriginalTime = Convert.ToInt32(time.TotalSeconds) * 60;

                timer->Time = timer->OriginalTime;
                timer->Start = timer->OriginalTime - 180;
            }
        }
    }
}
