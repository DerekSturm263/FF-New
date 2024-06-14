using Extensions.Components.Miscellaneous;
using Quantum;
using System.Linq;
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
        return GetItemCount(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public int GetItemCount(AssetGuid id)
    {
        return GetItemCount(new AssetRefInfoAsset() { Id = id });
    }

    public int GetItemCount(AssetRefInfoAsset assetRef)
    {
        return _inventory.CountItemCollection[assetRef];
    }

    public bool HasItem(InfoAssetAsset asset)
    {
        return HasItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public bool HasItem(AssetGuid id)
    {
        return HasItem(new AssetRefInfoAsset() { Id = id });
    }

    public bool HasItem(AssetRefInfoAsset assetRef)
    {
        return _inventory.ConditionalItemCollection[assetRef];
    }

    public void GainCountableItem(InfoAssetAsset asset, int count)
    {
        GainCountableItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid }, count);
    }

    public void GainCountableItem(AssetGuid id, int count)
    {
        GainCountableItem(new AssetRefInfoAsset() { Id = id }, count);
    }

    public void GainCountableItem(AssetRefInfoAsset assetRef, int count)
    {
        _inventory.ConditionalItemCollection.TryAdd(assetRef, true);

        if (!_inventory.CountItemCollection.ContainsKey(assetRef))
            _inventory.CountItemCollection[assetRef] = 0;

        _inventory.CountItemCollection[assetRef] += count;
    }

    public void GainConditionalItem(InfoAssetAsset asset)
    {
        GainConditionalItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public void GainConditionalItem(AssetGuid id)
    {
        GainConditionalItem(new AssetRefInfoAsset() { Id = id });
    }

    public void GainConditionalItem(AssetRefInfoAsset assetRef)
    {
        _inventory.ConditionalItemCollection.TryAdd(assetRef, true);
    }

    public void UseCountableItem(InfoAssetAsset asset)
    {
        UseCountableItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public void UseCountableItem(AssetGuid id)
    {
        UseCountableItem(new AssetRefInfoAsset() { Id = id });
    }

    public void UseCountableItem(AssetRefInfoAsset assetRef)
    {
        if (_inventory.CountItemCollection[assetRef] == -1)
            return;

        --_inventory.CountItemCollection[assetRef];
    }

    public void GainCurrency(int amount)
    {
        _inventory.Currency += amount;
        FindFirstObjectByType<DisplayCurrency>()?.UpdateDisplay(_inventory.Currency);
    }

    public void LoseCurrency(int amount)
    {
        _inventory.Currency -= amount;
        FindFirstObjectByType<DisplayCurrency>()?.UpdateDisplay(_inventory.Currency);
    }

    public bool HasEnoughCurrency(int amount)
    {
        return _inventory.Currency >= amount;
    }

    public bool HasEnoughCurrency(params int?[] amount)
    {
        return _inventory.Currency >= amount.Sum(item => item.GetValueOrDefault());
    }
}
