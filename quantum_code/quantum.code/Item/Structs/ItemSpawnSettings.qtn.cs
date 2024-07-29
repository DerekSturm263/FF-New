using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct ItemSpawnSettings
    {
        public static ItemSpawnSettings Lerp(ItemSpawnSettings a, ItemSpawnSettings b, FP t)
        {
            return new()
            {
                Item = a.Item,
                Velocity = FPVector2.Lerp(a.Velocity, b.Velocity, t),
                Offset = FPVector2.Lerp(a.Offset, b.Offset, t),
                StartHolding = a.StartHolding
            };
        }
    }
}
