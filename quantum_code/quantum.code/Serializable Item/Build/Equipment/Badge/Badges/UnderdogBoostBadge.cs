using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class UnderdogBoostBadge : Badge
    {
        public override void OnApply(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
            {

            }
        }

        public override void OnRemove(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
            {

            }
        }
    }
}
