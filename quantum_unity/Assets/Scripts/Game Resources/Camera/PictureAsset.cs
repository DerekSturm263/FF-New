using UnityEngine;

[CreateAssetMenu(fileName = "New Picture", menuName = "Fusion Fighters/Picture")]
public class PictureAsset : InfoAssetAsset
{
    public SerializableWrapper<Picture> Picture;
}
