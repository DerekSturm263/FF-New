using Extensions.Components.UI;
using Quantum;
using UnityEngine;

public class DisplayCurrency : DisplayText<int>
{
    protected override string GetInfo(int currency)
    {
        return currency.ToString();
    }

    protected override int GetValue() => InventoryController.Instance.Inventory.Currency;
}
