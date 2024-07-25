using Extensions.Components.UI;

public class DisplayCurrency : DisplayText<ulong>
{
    protected override string GetInfo(ulong currency)
    {
        return currency.ToString();
    }

    protected override ulong GetValue() => InventoryController.Instance.Inventory.Currency;
}
