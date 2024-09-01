using Extensions.Types;
using Quantum;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryAsset))]
public class InventoryAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InventoryAsset inventoryAsset = target as InventoryAsset;

        if (GUILayout.Button("Import Inventory"))
        {
            inventoryAsset.Inventory.ItemCollection = new();

            foreach (string path in inventoryAsset.ImportFolders)
            {
                foreach (InfoAssetAsset infoAsset in Resources.LoadAll<InfoAssetAsset>(path))
                {
                    inventoryAsset.Inventory.ItemCollection.Add(new() { Id = infoAsset.AssetObject.Identifier.Guid }, new(infoAsset.IsUnlocked, infoAsset.StartingCount));
                }
            }
        }

        if (GUILayout.Button("Set All Unlocked"))
        {
            inventoryAsset.Inventory.Currency = uint.MaxValue;

            Dictionary<AssetRefInfoAsset, Tuple<bool, Nullable<int>>> oldInventory = new(inventoryAsset.Inventory.ItemCollection);
            inventoryAsset.Inventory.ItemCollection = new();

            foreach (var item in oldInventory)
            {
                inventoryAsset.Inventory.ItemCollection.Add(item.Key, new(true, item.Value.Item2.HasValue ? 99 : new Nullable<int>()));
            }
        }

        GUILayout.Space(10);

        base.OnInspectorGUI();
    }
}
