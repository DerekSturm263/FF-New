using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Fusion Fighters/Inventory")]
public class InventoryAsset : ScriptableObject
{
    public System.Collections.Generic.List<string> ImportFolders;

    public Inventory Inventory;
}
