using Photon.Deterministic;
using System.Collections.Generic;

namespace Quantum.Types
{
    public static class ArrayHelper
    {
        public static FPVector2[] All(ArrayPlayerSpawnPoints array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4];
        }

        public static unsafe FPVector2*[] AllPtr(ArrayPlayerSpawnPoints* array)
        {
            return [&array->Item1, &array->Item2, &array->Item3, &array->Item4];
        }

        public static FPVector2[] All(ArrayItemSpawnPoints array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8, array.Item9, array.Item10, array.Item11, array.Item12, array.Item13, array.Item14, array.Item15, array.Item16];
        }

        public static unsafe FPVector2*[] AllPtr(ArrayItemSpawnPoints* array)
        {
            return [&array->Item1, &array->Item2, &array->Item3, &array->Item4, &array->Item5, &array->Item6, &array->Item7, &array->Item8, &array->Item9, &array->Item10, &array->Item11, &array->Item12, &array->Item13, &array->Item14, &array->Item15, &array->Item16];
        }

        public static List<AssetRefItem> All(ArrayItems array)
        {
            List<AssetRefItem> items = [];

            if (array.Item1.Id.IsValid)
                items.Add(array.Item1);

            if (array.Item2.Id.IsValid)
                items.Add(array.Item2);

            if (array.Item3.Id.IsValid)
                items.Add(array.Item3);

            if (array.Item4.Id.IsValid)
                items.Add(array.Item4);

            if (array.Item5.Id.IsValid)
                items.Add(array.Item5);

            if (array.Item6.Id.IsValid)
                items.Add(array.Item6);

            if (array.Item7.Id.IsValid)
                items.Add(array.Item7);

            if (array.Item8.Id.IsValid)
                items.Add(array.Item8);

            if (array.Item9.Id.IsValid)
                items.Add(array.Item9);

            if (array.Item10.Id.IsValid)
                items.Add(array.Item10);

            if (array.Item11.Id.IsValid)
                items.Add(array.Item11);

            if (array.Item12.Id.IsValid)
                items.Add(array.Item12);

            if (array.Item13.Id.IsValid)
                items.Add(array.Item13);

            if (array.Item14.Id.IsValid)
                items.Add(array.Item14);

            if (array.Item15.Id.IsValid)
                items.Add(array.Item15);

            if (array.Item16.Id.IsValid)
                items.Add(array.Item16);

            return items;
        }

        public static unsafe AssetRefItem*[] AllPtr(ArrayItems array)
        {
            AssetRefItem*[] items = new AssetRefItem*[16];

            if (array.Item1.Id.IsValid)
                items[0] = &array.Item1;

            if (array.Item2.Id.IsValid)
                items[1] = &array.Item2;

            if (array.Item3.Id.IsValid)
                items[2] = &array.Item3;

            if (array.Item4.Id.IsValid)
                items[3] = &array.Item4;

            if (array.Item5.Id.IsValid)
                items[4] = &array.Item5;

            if (array.Item6.Id.IsValid)
                items[5] = &array.Item6;

            if (array.Item7.Id.IsValid)
                items[6] = &array.Item7;

            if (array.Item8.Id.IsValid)
                items[7] = &array.Item8;

            if (array.Item9.Id.IsValid)
                items[8] = &array.Item9;

            if (array.Item10.Id.IsValid)
                items[9] = &array.Item10;

            if (array.Item11.Id.IsValid)
                items[10] = &array.Item11;

            if (array.Item12.Id.IsValid)
                items[11] = &array.Item12;

            if (array.Item13.Id.IsValid)
                items[12] = &array.Item13;

            if (array.Item14.Id.IsValid)
                items[13] = &array.Item14;

            if (array.Item15.Id.IsValid)
                items[14] = &array.Item15;

            if (array.Item16.Id.IsValid)
                items[15] = &array.Item16;

            return items;
        }

        public static void Set(ref ArrayItems destination, AssetRefItem[] source)
        {
            destination = default;

            if (source.Length == 0)
                return;
            destination.Item1 = source[0];

            if (source.Length == 1)
                return;
            destination.Item2 = source[1];

            if (source.Length == 2)
                return;
            destination.Item3 = source[2];

            if (source.Length == 3)
                return;
            destination.Item4 = source[3];

            if (source.Length == 4)
                return;
            destination.Item5 = source[4];

            if (source.Length == 5)
                return;
            destination.Item6 = source[5];

            if (source.Length == 6)
                return;
            destination.Item7 = source[6];

            if (source.Length == 7)
                return;
            destination.Item8 = source[7];

            if (source.Length == 8)
                return;
            destination.Item9 = source[8];

            if (source.Length == 9)
                return;
            destination.Item10 = source[9];

            if (source.Length == 10)
                return;
            destination.Item11 = source[10];

            if (source.Length == 11)
                return;
            destination.Item12 = source[11];

            if (source.Length == 12)
                return;
            destination.Item13 = source[12];

            if (source.Length == 13)
                return;
            destination.Item14 = source[13];

            if (source.Length == 14)
                return;
            destination.Item15 = source[14];

            if (source.Length == 15)
                return;
            destination.Item16 = source[15];
        }

        public static void Add(ref ArrayItems array, AssetRefItem item)
        {
            List<AssetRefItem> items = All(array);
            items.Add(item);

            Set(ref array, [.. items]);
        }

        public static void Remove(ref ArrayItems array, AssetRefItem item)
        {
            List<AssetRefItem> items = All(array);
            items.Remove(item);

            Set(ref array, [.. items]);
        }

        public static List<AssetRefStageAsset> All(ArrayStages array)
        {
            List<AssetRefStageAsset> stages = [];

            if (array.Item1.Id.IsValid)
                stages.Add(array.Item1);

            if (array.Item2.Id.IsValid)
                stages.Add(array.Item2);

            if (array.Item3.Id.IsValid)
                stages.Add(array.Item3);

            if (array.Item4.Id.IsValid)
                stages.Add(array.Item4);

            if (array.Item5.Id.IsValid)
                stages.Add(array.Item5);

            if (array.Item6.Id.IsValid)
                stages.Add(array.Item6);

            if (array.Item7.Id.IsValid)
                stages.Add(array.Item7);

            if (array.Item8.Id.IsValid)
                stages.Add(array.Item8);

            if (array.Item9.Id.IsValid)
                stages.Add(array.Item9);

            if (array.Item10.Id.IsValid)
                stages.Add(array.Item10);

            if (array.Item11.Id.IsValid)
                stages.Add(array.Item11);

            if (array.Item12.Id.IsValid)
                stages.Add(array.Item12);

            return stages;
        }

        public static unsafe AssetRefStageAsset*[] AllPtr(ArrayStages array)
        {
            AssetRefStageAsset*[] stages = new AssetRefStageAsset*[12];

            if (array.Item1.Id.IsValid)
                stages[0] = &array.Item1;

            if (array.Item2.Id.IsValid)
                stages[1] = &array.Item2;

            if (array.Item3.Id.IsValid)
                stages[2] = &array.Item3;

            if (array.Item4.Id.IsValid)
                stages[3] = &array.Item4;

            if (array.Item5.Id.IsValid)
                stages[4] = &array.Item5;

            if (array.Item6.Id.IsValid)
                stages[5] = &array.Item6;

            if (array.Item7.Id.IsValid)
                stages[6] = &array.Item7;

            if (array.Item8.Id.IsValid)
                stages[7] = &array.Item8;

            if (array.Item9.Id.IsValid)
                stages[8] = &array.Item9;

            if (array.Item10.Id.IsValid)
                stages[9] = &array.Item10;

            if (array.Item11.Id.IsValid)
                stages[10] = &array.Item11;

            if (array.Item12.Id.IsValid)
                stages[11] = &array.Item12;

            return stages;
        }

        public static void Set(ref ArrayStages destination, AssetRefStageAsset[] source)
        {
            destination = default;

            if (source.Length == 0)
                return;
            destination.Item1 = source[0];

            if (source.Length == 1)
                return;
            destination.Item2 = source[1];

            if (source.Length == 2)
                return;
            destination.Item3 = source[2];

            if (source.Length == 3)
                return;
            destination.Item4 = source[3];

            if (source.Length == 4)
                return;
            destination.Item5 = source[4];

            if (source.Length == 5)
                return;
            destination.Item6 = source[5];

            if (source.Length == 6)
                return;
            destination.Item7 = source[6];

            if (source.Length == 7)
                return;
            destination.Item8 = source[7];

            if (source.Length == 8)
                return;
            destination.Item9 = source[8];

            if (source.Length == 9)
                return;
            destination.Item10 = source[9];

            if (source.Length == 10)
                return;
            destination.Item11 = source[10];

            if (source.Length == 11)
                return;
            destination.Item12 = source[11];
        }

        public static void Add(ref ArrayStages array, AssetRefStageAsset item)
        {
            List<AssetRefStageAsset> items = All(array);
            items.Add(item);

            Set(ref array, [.. items]);
        }

        public static void Remove(ref ArrayStages array, AssetRefStageAsset item)
        {
            List<AssetRefStageAsset> items = All(array);
            items.Remove(item);

            Set(ref array, [.. items]);
        }

        public static StaticColliderInfo[] All(ArrayStaticColliderInfo array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8];
        }

        public static unsafe StaticColliderInfo*[] AllPtr(ArrayStaticColliderInfo* array)
        {
            return [&array->Item1, &array->Item2, &array->Item3, &array->Item4, &array->Item5, &array->Item6, &array->Item7, &array->Item8];
        }

        public static PositionalGizmo[] All(ArrayPositionalGizmoInfo array)
        {
            return [array.Item1, array.Item2, array.Item3, array.Item4, array.Item5, array.Item6, array.Item7, array.Item8, array.Item9, array.Item10, array.Item11, array.Item12, array.Item13, array.Item14, array.Item15, array.Item16];
        }

        public static unsafe PositionalGizmo*[] AllPtr(ArrayPositionalGizmoInfo* array)
        {
            return [&array->Item1, &array->Item2, &array->Item3, &array->Item4, &array->Item5, &array->Item6, &array->Item7, &array->Item8, &array->Item9, &array->Item10, &array->Item11, &array->Item12, &array->Item13, &array->Item14, &array->Item15, &array->Item16];
        }
    }
}
