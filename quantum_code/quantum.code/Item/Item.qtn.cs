using Photon.Deterministic;

namespace Quantum
{
    public partial class Item
    {
        public enum Handedness
        {
            OneHanded,
            TwoHanded
        }

        public string Name;
        public string Description;
        public Handedness HandednessType;
    }
}
