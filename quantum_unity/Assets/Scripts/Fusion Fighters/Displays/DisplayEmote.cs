using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = EmoteAsset;

public class DisplayEmote : DisplayTextAndImage<Type>
{
    [SerializeField] private int _direction;

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => _direction switch
    {
        0 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.Value.Cosmetics.Emotes.Up.Id),
        1 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.Value.Cosmetics.Emotes.Down.Id),
        2 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.Value.Cosmetics.Emotes.Left.Id),
        3 => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.Value.Cosmetics.Emotes.Right.Id),
        _ => default,
    };
}
