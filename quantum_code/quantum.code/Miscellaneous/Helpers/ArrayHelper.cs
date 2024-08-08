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

        public static void Set(ref ArrayTeams array, int index, Team value)
        {
            switch (index)
            {
                case 0:
                    array.Item1 = value;
                    break;

                case 1:
                    array.Item2 = value;
                    break;

                case 2:
                    array.Item3 = value;
                    break;

                case 3:
                    array.Item4 = value;
                    break;
            }
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

        public static IEnumerator<Team> Iterator(ArrayTeams array)
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

        public static Team[] All(ArrayTeams array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4];
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

        public static void Set(ref ArrayPlayerSpawnPoints array, int index, FPVector2 value)
        {
            switch (index)
            {
                case 0:
                    array.Item1 = value;
                    break;

                case 1:
                    array.Item2 = value;
                    break;

                case 2:
                    array.Item3 = value;
                    break;

                case 3:
                    array.Item4 = value;
                    break;
            }
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

        public static IEnumerator<FPVector2> Iterator(ArrayPlayerSpawnPoints array)
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

        public static FPVector2[] All(ArrayPlayerSpawnPoints array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4];
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

        public static void Set(ref ArrayItemSpawnPoints array, int index, FPVector2 value)
        {
            switch (index)
            {
                case 0:
                    array.Item1 = value;
                    break;

                case 1:
                    array.Item2 = value;
                    break;

                case 2:
                    array.Item3 = value;
                    break;

                case 3:
                    array.Item4 = value;
                    break;

                case 4:
                    array.Item5 = value;
                    break;

                case 5:
                    array.Item6 = value;
                    break;

                case 6:
                    array.Item7 = value;
                    break;

                case 7:
                    array.Item8 = value;
                    break;

                case 8:
                    array.Item9 = value;
                    break;

                case 9:
                    array.Item10 = value;
                    break;

                case 10:
                    array.Item11 = value;
                    break;

                case 11:
                    array.Item12 = value;
                    break;

                case 12:
                    array.Item13 = value;
                    break;

                case 13:
                    array.Item14 = value;
                    break;

                case 14:
                    array.Item15 = value;
                    break;

                case 15:
                    array.Item16 = value;
                    break;
            }
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

        public static IEnumerator<FPVector2> Iterator(ArrayItemSpawnPoints array)
        {
            for (int i = 0; i < 16; ++i)
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

        public static FPVector2[] All(ArrayItemSpawnPoints array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8, array.Item9, array.Item10, array.Item11, array.Item12, array.Item13, array.Item14, array.Item15, array.Item16];
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

        public static void Set(ref ArrayItems array, int index, AssetRefItem value)
        {
            switch (index)
            {
                case 0:
                    array.Item1 = value;
                    break;

                case 1:
                    array.Item2 = value;
                    break;

                case 2:
                    array.Item3 = value;
                    break;

                case 3:
                    array.Item4 = value;
                    break;

                case 4:
                    array.Item5 = value;
                    break;

                case 5:
                    array.Item6 = value;
                    break;

                case 6:
                    array.Item7 = value;
                    break;

                case 7:
                    array.Item8 = value;
                    break;

                case 8:
                    array.Item9 = value;
                    break;

                case 9:
                    array.Item10 = value;
                    break;

                case 10:
                    array.Item11 = value;
                    break;

                case 11:
                    array.Item12 = value;
                    break;

                case 12:
                    array.Item13 = value;
                    break;

                case 13:
                    array.Item14 = value;
                    break;

                case 14:
                    array.Item15 = value;
                    break;

                case 15:
                    array.Item16 = value;
                    break;
            }
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

        public static IEnumerator<AssetRefItem> Iterator(ArrayItems array)
        {
            for (int i = 0; i < 16; ++i)
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

        public static AssetRefItem[] All(ArrayItems array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8, array.Item9, array.Item10, array.Item11, array.Item12, array.Item13, array.Item14, array.Item15, array.Item16];
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

        public static void Set(ref ArrayStages array, int index, AssetRefStageAsset value)
        {
            switch (index)
            {
                case 0:
                    array.Item1 = value;
                    break;

                case 1:
                    array.Item2 = value;
                    break;

                case 2:
                    array.Item3 = value;
                    break;

                case 3:
                    array.Item4 = value;
                    break;

                case 4:
                    array.Item5 = value;
                    break;

                case 5:
                    array.Item6 = value;
                    break;

                case 6:
                    array.Item7 = value;
                    break;

                case 7:
                    array.Item8 = value;
                    break;

                case 8:
                    array.Item9 = value;
                    break;

                case 9:
                    array.Item10 = value;
                    break;

                case 10:
                    array.Item11 = value;
                    break;

                case 11:
                    array.Item12 = value;
                    break;
            }
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

        public static IEnumerator<AssetRefStageAsset> Iterator(ArrayStages array)
        {
            for (int i = 0; i < 12; ++i)
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

        public static AssetRefStageAsset[] All(ArrayStages array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8, array.Item9, array.Item10, array.Item11, array.Item12];
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

        public static void Set(ref ArrayStaticColliderInfo array, int index, StaticColliderInfo value)
        {
            switch (index)
            {
                case 0:
                    array.Item1 = value;
                    break;

                case 1:
                    array.Item2 = value;
                    break;

                case 2:
                    array.Item3 = value;
                    break;

                case 3:
                    array.Item4 = value;
                    break;

                case 4:
                    array.Item5 = value;
                    break;

                case 5:
                    array.Item6 = value;
                    break;

                case 6:
                    array.Item7 = value;
                    break;

                case 7:
                    array.Item8 = value;
                    break;
            }
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

        public static IEnumerator<StaticColliderInfo> Iterator(ArrayStaticColliderInfo array)
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

        public static StaticColliderInfo[] All(ArrayStaticColliderInfo array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8];
        }

        public static PositionalGizmo Get(ArrayPositionalGizmoInfo array, int index)
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

        public static void Set(ref ArrayPositionalGizmoInfo array, int index, PositionalGizmo value)
        {
            switch (index)
            {
                case 0:
                    array.Item1 = value;
                    break;

                case 1:
                    array.Item2 = value;
                    break;

                case 2:
                    array.Item3 = value;
                    break;

                case 3:
                    array.Item4 = value;
                    break;

                case 4:
                    array.Item5 = value;
                    break;

                case 5:
                    array.Item6 = value;
                    break;

                case 6:
                    array.Item7 = value;
                    break;

                case 7:
                    array.Item8 = value;
                    break;

                case 8:
                    array.Item9 = value;
                    break;

                case 9:
                    array.Item10 = value;
                    break;

                case 10:
                    array.Item11 = value;
                    break;

                case 11:
                    array.Item12 = value;
                    break;

                case 12:
                    array.Item13 = value;
                    break;

                case 13:
                    array.Item14 = value;
                    break;

                case 14:
                    array.Item15 = value;
                    break;

                case 15:
                    array.Item16 = value;
                    break;
            }
        }

        public static unsafe PositionalGizmo* GetPointer(ArrayPositionalGizmoInfo array, int index)
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

        public static IEnumerator<PositionalGizmo> Iterator(ArrayPositionalGizmoInfo array)
        {
            for (int i = 0; i < 16; ++i)
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

        public static PositionalGizmo[] All(ArrayPositionalGizmoInfo array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8, array.Item9, array.Item10, array.Item11, array.Item12, array.Item13, array.Item14, array.Item15, array.Item16];
        }
    }
}
