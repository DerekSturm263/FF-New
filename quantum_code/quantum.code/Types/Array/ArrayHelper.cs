using Photon.Deterministic;

namespace Quantum.Types
{
    public static class ArrayHelper
    {
        public static FPVector2 Get(Array4FPVector2 array, int index)
        {
            return index switch
            {
                0 => array.Item1,
                1 => array.Item2,
                2 => array.Item3,
                3 => array.Item4,
                _ => default
            };
        }
    }
}