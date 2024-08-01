public class SubTemplatePopulator : PopulateAsset<SubTemplateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Subs/Templates";

    protected override bool DoSpawn(SubTemplateAsset item) => item.IncludeInLists && InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(SubTemplateAsset item) => false;
}
