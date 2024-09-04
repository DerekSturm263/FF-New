using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct HurtboxSettings
    {
        public static HurtboxSettings Default = new()
        {
            CanBeDamaged = true,
            CanBeInterrupted = true,
            CanBeKnockedBack = true,
            DamageToBreak = 0
        };

        public static HurtboxSettings Intangible = new()
        {
            CanBeDamaged = false,
            CanBeInterrupted = false,
            CanBeKnockedBack = false,
            DamageToBreak = int.MaxValue
        };
    }
}
