using Photon.Deterministic;
using Quantum.Movement;
using System.Text.RegularExpressions;

namespace Quantum
{
    public unsafe class MatchSystem : SystemMainThreadFilter<MatchSystem.Filter>
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

            f.SystemEnable<MovementSystem>();
        }

        public static void EndOfMatch(Frame f)
        {
            f.Global->DeltaTime = (FP._1 / f.UpdateRate) * FP._0_25;
        }
    }
}
