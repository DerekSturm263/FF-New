using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = VoiceAsset;

public class DisplayVoice : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentBuild.value.Cosmetics.Voice.Id);
}
