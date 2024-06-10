using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public partial class WeaponTemplate : InfoAsset
    {
        // Main
        public DirectionalMoveRef Primaries;
        public DirectionalMoveRef Aerials;
        
        // Support
        public DirectionalMoveRef Secondaries;
        public DirectionalMoveRef EnergySecondaries;
    }
}
