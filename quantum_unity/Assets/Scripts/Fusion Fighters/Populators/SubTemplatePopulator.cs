public class SubTemplatePopulator : PopulateAsset<SubTemplateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Subs/Templates";

    protected override bool DoSpawn(SubTemplateAsset item) => InventoryController.Instance.HasUnlockedItem(item);
}
