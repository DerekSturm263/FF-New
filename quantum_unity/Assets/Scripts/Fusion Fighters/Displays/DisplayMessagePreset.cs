using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = MessagePresetAsset;

public class DisplayMessagePreset : DisplayTextAndImage<Type>
{
    public enum ParentType
    {
        EmoteUp,
        EmoteDown,
        EmoteLeft,
        EmoteRight,
    }

    [SerializeField] private ParentType _type;

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue()
    {
        AssetGuid id = _type switch
        {
            ParentType.EmoteUp => BuildController.Instance.CurrentBuild.value.Emotes.Up.Message.Id,
            ParentType.EmoteDown => BuildController.Instance.CurrentBuild.value.Emotes.Down.Message.Id,
            ParentType.EmoteLeft => BuildController.Instance.CurrentBuild.value.Emotes.Left.Message.Id,
            ParentType.EmoteRight => BuildController.Instance.CurrentBuild.value.Emotes.Right.Message.Id,
            _ => default
        };

        return UnityDB.FindAsset<Type>(id);
    }
}
