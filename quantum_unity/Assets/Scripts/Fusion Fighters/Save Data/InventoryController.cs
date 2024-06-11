using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class InventoryController : Controller<InventoryController>
{
    [SerializeField] private Inventory _default;
    public Inventory Default => _default;

    private Inventory _inventory;
    public Inventory Inventory => _inventory;

    [System.NonSerialized] private bool _isInitialized = false;

    public override void Initialize()
    {
        base.Initialize();

        if (Serializer.TryLoadAs($"{Application.persistentDataPath}/Inventory.json", $"{Application.persistentDataPath}", out Inventory inventory))
            _inventory = inventory;
        else
            _inventory = _default;

        if (!_isInitialized)
        {
            Application.quitting += Shutdown;
            _isInitialized = true;
        }
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _isInitialized = false;

        Serializer.Save(_inventory, "Inventory", $"{Application.persistentDataPath}");

        base.Shutdown();
    }

    public int GetItemCount(InfoAssetAsset asset)
    {
        AssetRefInfoAsset assetRef = new() { Id = asset.AssetObject.Guid };
        return _inventory.CountItemCollection[assetRef];
    }

    public bool HasItem(InfoAssetAsset asset)
    {
        AssetRefInfoAsset assetRef = new() { Id = asset.AssetObject.Guid };
        return _inventory.ConditionalItemCollection[assetRef];
    }

    public void GetCountableItem(InfoAssetAsset asset, int count)
    {
        AssetRefInfoAsset assetRef = new() { Id = asset.AssetObject.Guid };
        _inventory.ConditionalItemCollection.TryAdd(assetRef, true);

        if (!_inventory.CountItemCollection.ContainsKey(assetRef))
            _inventory.CountItemCollection[assetRef] = 0;

        _inventory.CountItemCollection[assetRef] += count;
    }

    public void GetConditionalItem(InfoAssetAsset asset)
    {
        AssetRefInfoAsset assetRef = new() { Id = asset.AssetObject.Guid };
        _inventory.ConditionalItemCollection.TryAdd(assetRef, true);
    }

    public void UseCountableItem(InfoAssetAsset asset)
    {
        AssetRefInfoAsset assetRef = new() { Id = asset.AssetObject.Guid };
        --_inventory.CountItemCollection[assetRef];
    }

    public void GainCurrency(int amount)
    {
        _inventory.Currency += amount;
    }

    public void LoseCurrency(int amount)
    {
        _inventory.Currency -= amount;
    }

    public bool HasEnoughCurrency(int amount)
    {
        return _inventory.Currency >= amount;
    }

    public bool HasEnoughCurrency(params int[] amount)
    {
        for (int i = 0; i < amount.Length; ++i)
        {
            if (!HasEnoughCurrency(amount[i]))
                return false;
        }

        return true;
    }
}
