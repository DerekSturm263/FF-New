namespace Quantum
{
    [System.Serializable]
    public partial class WeaponTemplate : InfoAsset
    {
        public AssetRefEntityPrototype Weapon;

        // Primary
        public DirectionalMoveRef Primaries;
        public DirectionalAerialMoveRef Aerials;

        public MoveRef Dash;
        
        // Secondary
        public DirectionalMoveRef Secondaries;
    }
}
