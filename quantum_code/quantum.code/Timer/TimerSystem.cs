using Photon.Deterministic;
using Quantum.Multiplayer;

namespace Quantum.Timing
{
    public unsafe class TimerSystem : SystemMainThreadFilter<TimerSystem.Filter>
    {
        public override bool StartEnabled => false;

        public struct Filter
        {
            public EntityRef Entity;
            public Timer* Timer;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (!filter.Timer->IsCounting)
                return;
            
            filter.Timer->Time += f.DeltaTime * filter.Timer->Multiplier;
            FP remainder = filter.Timer->Time - filter.Timer->Time.AsInt;
            
            if (remainder <= (FP)1 / f.UpdateRate)
            {
                f.Events.OnTimerTick(filter.Entity, filter.Timer->Time.AsInt);

                if (filter.Timer->Time.AsInt == filter.Timer->End)
                {
                    filter.Timer->IsCounting = false;

                    if (filter.Timer->TriggerStartOfMatch)
                    {
                        MatchSystem.StartOfMatch(f);
                    }

                    if (filter.Timer->TriggerEndOfMatch)
                    {
                        MatchSystem.EndOfMatch(f);
                    }
                }

                if (filter.Timer->TriggerEndOfMatch && filter.Timer->Time.AsInt == 61)
                    f.Events.OnOneMinuteLeft();
            }

            if (filter.Timer->TriggerStartOfMatch)
            {
                foreach (var playerLink in f.Unsafe.GetComponentBlockIterator<PlayerLink>())
                {
                    if (f.Unsafe.TryGetPointer(playerLink.Entity, out Stats* stats))
                    {
                        FP lerpValue = filter.Timer->Time / filter.Timer->OriginalTime;

                        FP health = FPMath.Lerp(stats->MaxHealth, 0, lerpValue);
                        StatsSystem.SetHealth(f, playerLink.Component, stats, health);

                        FP energy = FPMath.Lerp(stats->MaxEnergy / 5, 0, lerpValue);
                        StatsSystem.SetEnergy(f, playerLink.Component, stats, energy);

                        int stocks = FPMath.Lerp(stats->MaxStocks, 0, lerpValue).AsInt;
                        StatsSystem.SetStocks(f, playerLink.Component, stats, stocks);
                    }
                }
            }
        }

        public override void OnEnabled(Frame f)
        {
            foreach (var timer in f.Unsafe.GetComponentBlockIterator<Timer>())
            {
                timer.Component->Time = timer.Component->OriginalTime;
                f.Events.OnTimerTick(timer.Entity, timer.Component->Time.AsInt);
            }
        }
    }
}
