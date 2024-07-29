namespace Quantum
{
    [System.Serializable]
    public partial class WeaponTemplate : InfoAsset
    {
        public AssetRefEntityPrototype Weapon;

        // Primary
        public DirectionalMoveRef Primaries;
        public DirectionalMoveRef Aerials;
        
        // Secondary
        public DirectionalMoveRef Secondaries;
    }
}
