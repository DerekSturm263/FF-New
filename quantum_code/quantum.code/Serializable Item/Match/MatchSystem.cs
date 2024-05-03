using Photon.Deterministic;
using Quantum.Movement;
using System.Text.RegularExpressions;

namespace Quantum
{
    public unsafe class MatchSystem : SystemMainThreadFilter<MatchSystem.Filter>, ISignalOnComponentAdded<MatchInstance>, ISignalOnComponentRemoved<MatchInstance>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public MatchInstance* MatchInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var teamList = f.ResolveList(filter.MatchInstance->Match.Teams);
            
            foreach (Team team in teamList)
            {

            }
        }

        public static void StartOfMatch(Frame f)
        {
            if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
            {
                playerCounter->CanPlayersEdit = false;
            }

            foreach (var playerLink in f.Unsafe.GetComponentBlockIterator<PlayerLink>())
            {
                if (f.Unsafe.TryGetPointer(playerLink.Entity, out Stats* stats))
                {
                    StatsSystem.SetHealth(f, playerLink.Component, stats, stats->MaxHealth);
                    StatsSystem.SetEnergy(f, playerLink.Component, stats, stats->MaxEnergy / 5);
                    StatsSystem.SetStocks(f, playerLink.Component, stats, stats->MaxStocks);
                }
            }

            f.SystemEnable<PlayerStateSystem>();
            f.SystemEnable<ItemSpawnSystem>();
        }

        public static void EndOfMatch(Frame f)
        {
            f.Global->DeltaTime = (FP._1 / f.UpdateRate) * FP._0_25;
        }

        public void OnAdded(Frame f, EntityRef entity, MatchInstance* component)
        {
            component->Match.Teams = f.AllocateList<Team>();
        }

        public void OnRemoved(Frame f, EntityRef entity, MatchInstance* component)
        {
            f.FreeList(component->Match.Teams);
            component->Match.Teams = default;
        }
    }
}
