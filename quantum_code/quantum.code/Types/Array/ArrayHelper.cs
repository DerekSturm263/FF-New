using Photon.Deterministic;
using System.Collections.Generic;

namespace Quantum.Types
{
    public static class ArrayHelper
    {
        public static Team Get(ArrayTeams array, int index)
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

        public static unsafe Team* GetPointer(ArrayTeams array, int index)
        {
            return index switch
            {
                0 => &array.Item1,
                1 => &array.Item2,
                2 => &array.Item3,
                3 => &array.Item4,
                _ => default
            };
        }

        public static IEnumerator<Team> All(ArrayTeams array)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return i switch
                {
                    0 => array.Item1,
                    1 => array.Item2,
                    2 => array.Item3,
                    3 => array.Item4,
                    _ => default
                };
            }
        }

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

        public static unsafe FPVector2* GetPointer(ArrayPlayerSpawnPoints array, int index)
        {
            return index switch
            {
                0 => &array.Item1,
                1 => &array.Item2,
                2 => &array.Item3,
                3 => &array.Item4,
                _ => default
            };
        }

        public static IEnumerator<FPVector2> All(ArrayPlayerSpawnPoints array)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return i switch
                {
                    0 => array.Item1,
                    1 => array.Item2,
                    2 => array.Item3,
                    3 => array.Item4,
                    _ => default
                };
            }
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

        public static unsafe FPVector2* GetPointer(ArrayItemSpawnPoints array, int index)
        {
            return index switch
            {
                0 => &array.Item1,
                1 => &array.Item2,
                2 => &array.Item3,
                3 => &array.Item4,
                4 => &array.Item5,
                5 => &array.Item6,
                6 => &array.Item7,
                7 => &array.Item8,
                8 => &array.Item9,
                9 => &array.Item10,
                10 => &array.Item11,
                11 => &array.Item12,
                12 => &array.Item13,
                13 => &array.Item14,
                14 => &array.Item15,
                15 => &array.Item16,
                _ => default
            };
        }

        public static IEnumerator<FPVector2> All(ArrayItemSpawnPoints array)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return i switch
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
        }

        public static AssetRefItem Get(ArrayItems array, int index)
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

        public static unsafe AssetRefItem* GetPointer(ArrayItems array, int index)
        {
            return index switch
            {
                0 => &array.Item1,
                1 => &array.Item2,
                2 => &array.Item3,
                3 => &array.Item4,
                4 => &array.Item5,
                5 => &array.Item6,
                6 => &array.Item7,
                7 => &array.Item8,
                8 => &array.Item9,
                9 => &array.Item10,
                10 => &array.Item11,
                11 => &array.Item12,
                12 => &array.Item13,
                13 => &array.Item14,
                14 => &array.Item15,
                15 => &array.Item16,
                _ => default
            };
        }

        public static IEnumerator<AssetRefItem> All(ArrayItems array)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return i switch
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
        }

        public static AssetRefStageAsset Get(ArrayStages array, int index)
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

        public static unsafe AssetRefStageAsset* GetPointer(ArrayStages array, int index)
        {
            return index switch
            {
                0 => &array.Item1,
                1 => &array.Item2,
                2 => &array.Item3,
                3 => &array.Item4,
                4 => &array.Item5,
                5 => &array.Item6,
                6 => &array.Item7,
                7 => &array.Item8,
                8 => &array.Item9,
                9 => &array.Item10,
                10 => &array.Item11,
                11 => &array.Item12,
                _ => default
            };
        }

        public static IEnumerator<AssetRefStageAsset> All(ArrayStages array)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return i switch
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

        public static StaticColliderInfo Get(ArrayStaticColliderInfo array, int index)
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
                _ => default
            };
        }

        public static unsafe StaticColliderInfo* GetPointer(ArrayStaticColliderInfo array, int index)
        {
            return index switch
            {
                0 => &array.Item1,
                1 => &array.Item2,
                2 => &array.Item3,
                3 => &array.Item4,
                4 => &array.Item5,
                5 => &array.Item6,
                6 => &array.Item7,
                7 => &array.Item8,
                _ => default
            };
        }

        public static IEnumerator<StaticColliderInfo> All(ArrayStaticColliderInfo array)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return i switch
                {
                    0 => array.Item1,
                    1 => array.Item2,
                    2 => array.Item3,
                    3 => array.Item4,
                    4 => array.Item5,
                    5 => array.Item6,
                    6 => array.Item7,
                    7 => array.Item8,
                    _ => default
                };
            }
        }
    }
}
