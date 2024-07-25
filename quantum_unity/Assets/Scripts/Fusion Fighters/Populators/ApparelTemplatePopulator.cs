public class ApparelTemplatePopulator : PopulateAsset<ApparelTemplateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Apparel/Templates";

    protected override bool DoSpawn(ApparelTemplateAsset item) => InventoryController.Instance.HasUnlockedItem(item);
}
