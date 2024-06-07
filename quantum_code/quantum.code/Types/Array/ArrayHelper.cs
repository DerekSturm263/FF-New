using Photon.Deterministic;

namespace Quantum.Types
{
    public static class ArrayHelper
    {
        public static FPVector2 Get(ArrayPlayerSpawnPoints array, int index)
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
        
        public static FPVector2 Get(ArrayItemSpawnPoints array, int index)
        {
            return index switch
            {
                0 => array.Item1,
                1 => array.Item2,
                2 => array.Item3,
                3 => array.Item4,
                4 => array.Item5,
                5 => array.Item6,
                6 => array.Item7,
                7 => array.Item8,
                8 => array.Item9,
                9 => array.Item10,
                10 => array.Item11,
                11 => array.Item12,
                12 => array.Item13,
                13 => array.Item14,
                14 => array.Item15,
                15 => array.Item16,
                _ => default
            };
        }
        
        public static FPVector2 Get(ArrayItem array, int index)
        {
            return index switch
            {
                0 => array.Item1,
                1 => array.Item2,
                2 => array.Item3,
                3 => array.Item4,
                4 => array.Item5,
                5 => array.Item6,
                6 => array.Item7,
                7 => array.Item8,
                8 => array.Item9,
                9 => array.Item10,
                10 => array.Item11,
                11 => array.Item12,
                12 => array.Item13,
                13 => array.Item14,
                14 => array.Item15,
                15 => array.Item16,
                _ => default
            };
        }
        
        public static FPVector2 Get(ArrayStage array, int index)
        {
            return index switch
            {
                0 => array.Item1,
                1 => array.Item2,
                2 => array.Item3,
                3 => array.Item4,
                4 => array.Item5,
                5 => array.Item6,
                6 => array.Item7,
                7 => array.Item8,
                8 => array.Item9,
                9 => array.Item10,
                10 => array.Item11,
                11 => array.Item12,
                _ => default
            };
        }
    }
}
