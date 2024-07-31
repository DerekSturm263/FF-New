using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = EmoteAsset;

public class DisplayEmote : DisplayTextAndImage<Type>
{
    [SerializeField] private int _direction;

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => _direction switch
    {
        0 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Up.Id),
        1 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Down.Id),
        2 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Left.Id),
        3 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Right.Id),
        _ => default,
    };
}
