
namespace Quantum
{
    [System.Serializable]
    public unsafe partial class VampiricWeaponEnhancer : WeaponEnhancer
    {
        public override void OnHit(Frame f, EntityRef user, EntityRef target)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->CurrentStats.Health += 20;
            }
        }
    }
}
