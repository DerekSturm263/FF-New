using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetRuleset : DeterministicCommand
    {
        public Ruleset ruleset;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref ruleset);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Ruleset applied!");

            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                if (f.Unsafe.TryGetPointer(stats.Entity, out Stats* statValues))
                {
                    f.Events.OnPlayerModifyHealth(stats.Entity, stats.Component.Index, statValues->CurrentStats.Health, statValues->CurrentStats.Health, ruleset.Players.MaxHealth);
                    f.Events.OnPlayerModifyEnergy(stats.Entity, stats.Component.Index, statValues->CurrentStats.Energy, statValues->CurrentStats.Energy, ruleset.Players.MaxEnergy);
                    f.Events.OnPlayerModifyStocks(stats.Entity, stats.Component.Index, statValues->CurrentStats.Stocks, statValues->CurrentStats.Stocks, ruleset.Players.StockCount);
                }
            }

            MatchSystem.SetRuleset(f, ruleset);
        }
    }
}
