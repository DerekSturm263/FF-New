using Photon.Deterministic;

namespace Quantum
{
    public abstract unsafe partial class Item
    {
        public enum Handedness
        {
            OneHanded,
            TwoHanded
        }

        public Handedness HandednessType;
        public int Uses;

        public abstract void Invoke(Frame f, PlayerLink* user, PlayerLink* target);
    }
}
