using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public partial class ApparelTemplate : InfoAsset
    {
        [System.Flags]
        public enum ApparelType
        {
            Headgear = 1 << 0,
            Clothing = 1 << 1,
            Legwear = 1 << 2
        }

        public ApparelType Type;
    }
}
