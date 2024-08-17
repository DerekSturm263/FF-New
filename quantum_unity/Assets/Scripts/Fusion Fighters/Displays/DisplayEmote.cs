using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = EmoteAsset;

public class DisplayEmote : DisplayTextAndImage<Type>
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private Direction _direction;

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => _direction switch
    {
        Direction.Up => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentBuild.value.Emotes.Up.Emote.Id),
        Direction.Down => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentBuild.value.Emotes.Down.Emote.Id),
        Direction.Left => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentBuild.value.Emotes.Left.Emote.Id),
        Direction.Right => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentBuild.value.Emotes.Right.Emote.Id),
        _ => default,
    };
}
