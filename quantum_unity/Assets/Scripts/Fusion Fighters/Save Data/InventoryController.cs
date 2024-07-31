using Extensions.Components.Miscellaneous;
using Quantum;
using System.Linq;
using UnityEngine;

public class InventoryController : Controller<InventoryController>
{
    [SerializeField] private InventoryAsset _default;

    private Inventory _inventory;
    public Inventory Inventory => _inventory;

    [System.NonSerialized] private bool _isInitialized = false;

    public override void Initialize()
    {
        base.Initialize();

        if (!_isInitialized)
        {
            if (FusionFighters.Serializer.TryLoadAs($"{Application.persistentDataPath}/SaveData/Misc/Inventory.json", $"{Application.persistentDataPath}/SaveData/Misc", out Inventory inventory))
                _inventory = inventory.DeepCopy();
            else
                _inventory = _default.Inventory.DeepCopy();

            Application.quitting += Shutdown;
            _isInitialized = true;
        }
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _isInitialized = false;

        FusionFighters.Serializer.Save(_inventory, "Inventory", $"{Application.persistentDataPath}/SaveData/Misc");

        base.Shutdown();
    }

    public bool HasUnlockedItem(InfoAssetAsset asset)
    {
        if (!asset || !asset.AssetObject.Guid.IsValid)
            return false;

        return HasUnlockedItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public bool HasUnlockedItem(AssetGuid id)
    {
        if (!id.IsValid)
            return false;

        return HasUnlockedItem(new AssetRefInfoAsset() { Id = id });
    }

    public bool HasUnlockedItem(AssetRefInfoAsset assetRef)
    {
        if (!assetRef.Id.IsValid || !_inventory.ItemCollection.ContainsKey(assetRef))
            return false;

        return _inventory.ItemCollection[assetRef].Item1;
    }

    public int GetItemCount(InfoAssetAsset asset)
    {
        if (!asset || !asset.AssetObject.Guid.IsValid)
            return 0;

        return GetItemCount(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public int GetItemCount(AssetGuid id)
    {
        if (!id.IsValid)
            return 0;

        return GetItemCount(new AssetRefInfoAsset() { Id = id });
    }

    public int GetItemCount(AssetRefInfoAsset assetRef)
    {
        if (!assetRef.Id.IsValid || !_inventory.ItemCollection.ContainsKey(assetRef))
            return 0;

        return _inventory.ItemCollection[assetRef].Item2.GetValueOrDefault();
    }

    public bool HasItem(InfoAssetAsset asset)
    {
        if (!asset || !asset.AssetObject.Guid.IsValid)
            return false;

        return HasItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public bool HasItem(AssetGuid id)
    {
        if (!id.IsValid)
            return false;

        return HasItem(new AssetRefInfoAsset() { Id = id });
    }

    public bool HasItem(AssetRefInfoAsset assetRef)
    {
        if (!assetRef.Id.IsValid || !_inventory.ItemCollection.ContainsKey(assetRef))
            return false;

        return _inventory.ItemCollection[assetRef].Item2.GetValueOrDefault(1) > 0;
    }

    public bool HasInfiniteItem(InfoAssetAsset asset)
    {
        if (!asset || !asset.AssetObject.Guid.IsValid)
            return false;

        return HasInfiniteItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid });
    }

    public bool HasInfiniteItem(AssetGuid id)
    {
        if (!id.IsValid)
            return false;

        return HasInfiniteItem(new AssetRefInfoAsset() { Id = id });
    }

    public bool HasInfiniteItem(AssetRefInfoAsset assetRef)
    {
        if (!assetRef.Id.IsValid || !_inventory.ItemCollection.ContainsKey(assetRef))
            return false;

        return !_inventory.ItemCollection[assetRef].Item2.HasValue;
    }

    public bool GainItem(InfoAssetAsset asset, int count)
    {
        if (!asset || !asset.AssetObject.Guid.IsValid)
            return false;

        return GainItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid }, count);
    }

    public bool GainItem(AssetGuid id, int count)
    {
        if (!id.IsValid)
            return false;

        return GainItem(new AssetRefInfoAsset() { Id = id }, count);
    }

    public bool GainItem(AssetRefInfoAsset assetRef, int count)
    {
        if (!assetRef.Id.IsValid || !_inventory.ItemCollection.ContainsKey(assetRef) || !_inventory.ItemCollection[assetRef].Item2.HasValue)
            return false;

        _inventory.ItemCollection[assetRef] = new(_inventory.ItemCollection[assetRef].Item1, _inventory.ItemCollection[assetRef].Item2.Value + count);
        return true;
    }

    public bool LoseItem(InfoAssetAsset asset, int count)
    {
        if (!asset || !asset.AssetObject.Guid.IsValid)
            return false;

        return LoseItem(new AssetRefInfoAsset() { Id = asset.AssetObject.Guid }, count);
    }

    public bool LoseItem(AssetGuid id, int count)
    {
        if (!id.IsValid)
            return false;

        return LoseItem(new AssetRefInfoAsset() { Id = id }, count);
    }

    public bool LoseItem(AssetRefInfoAsset assetRef, int count)
    {
        if (!assetRef.Id.IsValid || !_inventory.ItemCollection.ContainsKey(assetRef) || !_inventory.ItemCollection[assetRef].Item2.HasValue)
            return false;

        _inventory.ItemCollection[assetRef] = new(_inventory.ItemCollection[assetRef].Item1, _inventory.ItemCollection[assetRef].Item2.Value - count);
        return true;
    }

    public void GainCurrency(uint amount)
    {
        _inventory.Currency += amount;
        FindFirstObjectByType<DisplayCurrency>()?.UpdateDisplay(_inventory.Currency);
    }

    public void LoseCurrency(uint amount)
    {
        _inventory.Currency -= amount;
        FindFirstObjectByType<DisplayCurrency>()?.UpdateDisplay(_inventory.Currency);
    }

    public bool HasEnoughCurrency(uint amount)
    {
        return _inventory.Currency >= amount;
    }

    public bool HasEnoughCurrency(params uint?[] amount)
    {
        return _inventory.Currency >= (uint)amount.Sum(item => item.GetValueOrDefault());
    }

    public bool HasEnoughCurrency(params uint[] amount)
    {
        return _inventory.Currency >= (uint)amount.Sum(item => item);
    }
}
