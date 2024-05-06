using Photon.Deterministic;

namespace Quantum
{
    public partial class MainWeaponTemplate
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
