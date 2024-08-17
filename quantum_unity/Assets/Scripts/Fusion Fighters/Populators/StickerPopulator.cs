public class StickerPopulator : PopulateAsset<StickerAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Stickers";

    protected override bool IsEquipped(StickerAsset item) => false;
}
