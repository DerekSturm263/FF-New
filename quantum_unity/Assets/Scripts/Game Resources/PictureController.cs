using Extensions.Components.Miscellaneous;
using UnityEngine;

public class PictureController : Controller<PictureController>
{
    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Media/Pictures";

    [SerializeField] private SerializableWrapper<Picture> _currentSelected;
    public SerializableWrapper<Picture> CurrentSelected => _currentSelected;

    public void Select(SerializableWrapper<Picture> picture)
    {
        _currentSelected = picture;
    }
}
