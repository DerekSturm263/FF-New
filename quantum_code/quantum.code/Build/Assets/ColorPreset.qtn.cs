using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public partial class ColorPreset : InfoAsset
    {
        public ColorRGBA Color;
        public FP Smoothness = 1;
        public FP Metallic = 1;
        public FP FresnelStrength = 1;
    }
}
