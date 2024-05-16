using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public partial class WeaponTemplate : InfoAsset
    {
        // Main
        public DirectionalMoveRef GroundedHeavies;
        public DirectionalMoveRef GroundedLights;
        
        public DirectionalMoveRef Aerials;
        
        public MoveRef GetUp;
        public MoveRef Dash;
        
        // Support
        public DirectionalMoveRef GroundedSingles;
        public DirectionalMoveRef GroundedDoubles;
    }
}
