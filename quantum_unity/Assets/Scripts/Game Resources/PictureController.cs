using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using UnityEngine;

public class PictureController : Controller<PictureController>
{
    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Media/Pictures";

    private SerializableWrapper<Picture> _currentSelected;
    public SerializableWrapper<Picture> CurrentSelected => _currentSelected;

    public void Select(SerializableWrapper<Picture> picture)
    {
        _currentSelected = picture;
    }

    public void DeleteSelected()
    {
        _currentSelected.Delete();

        if (PicturePopulator.Instance && PicturePopulator.Instance.TryGetButtonFromItem(_currentSelected, out GameObject button))
        {
            DestroyImmediate(button);
            PicturePopulator.Instance.GetComponentInParent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
        }

        ToastController.Instance.Spawn("Picture deleted");
    }
}
