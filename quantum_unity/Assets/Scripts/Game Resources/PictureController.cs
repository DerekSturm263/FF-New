using Extensions.Components.Miscellaneous;
using Extensions.Miscellaneous;
using GameResources.UI.Popup;
using System;
using UnityEngine;

public class PictureController : Controller<PictureController>
{
    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Media/Pictures";

    private SerializableWrapper<Picture> _currentSelected;
    public SerializableWrapper<Picture> CurrentSelected => _currentSelected;

    private void Awake()
    {
        Initialize();
    }

    public void Select(SerializableWrapper<Picture> picture)
    {
        _currentSelected = picture;
    }

    public void DeleteSelected()
    {
        _instance._currentSelected.Delete();

        if (PicturePopulator.Instance && PicturePopulator.Instance.TryGetButtonFromItem(_instance._currentSelected, out GameObject button))
        {
            DestroyImmediate(button);
            PicturePopulator.Instance.GetComponentInParent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
        }

        ToastController.Instance.Spawn("Picture deleted");
    }

    public void ExportSelected()
    {
        Sprite picture = Helper.SpriteFromScreenshot($"{GetPath()}/{_instance._currentSelected.FileID}_PICTURE.png", 3840, 2160, TextureFormat.RGBA32, true, Texture2D.whiteTexture);
        byte[] renderBytes = picture.texture.EncodeToPNG();

        string pictureDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string fullPath = $"{pictureDirectory}/{_instance._currentSelected.CreationDate.ToFileTimeUtc()}.png";
        System.IO.File.WriteAllBytes(fullPath, renderBytes);

        ToastController.Instance.Spawn($"Exported to {fullPath}");
    }
}
