public class TilePopulator : PopulateAsset<TileAsset>
{
    protected override string FilePath() => "DB/Assets/Stage/Tiles";

    protected override bool IsEquipped(TileAsset item) => false;
}
