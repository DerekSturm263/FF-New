public class PicturePopulator : PopulateSerializable<Picture, PictureAsset>
{
    protected override string BuiltInFilePath() => "DB/Assets/Picture/Pictures";
    protected override string CustomFilePath() => PictureController.GetPath();

    protected override SerializableWrapper<Picture> GetFromBuiltInAsset(PictureAsset asset)
    {
        var item = asset.Picture;

        return item;
    }

    protected override bool IsEquipped(SerializableWrapper<Picture> item) => false;
    protected override bool IsNone(SerializableWrapper<Picture> item) => false;
}
