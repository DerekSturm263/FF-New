public class SubTemplatePopulator : PopulateAsset<SubTemplateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Subs/Templates";

    protected override bool IsEquipped(SubTemplateAsset item) => false;
}
