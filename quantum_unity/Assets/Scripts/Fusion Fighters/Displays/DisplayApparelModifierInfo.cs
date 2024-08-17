using Extensions.Components.UI;
using Extensions.Miscellaneous;
using Extensions.Types;
using UnityEngine;
using UnityEngine.UI;
using Type = ApparelModifierAsset;

public class DisplayApparelModifierInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        if (!item)
            return new(string.Empty, null);

        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => default;

    public void UpdateFromIndex(int index)
    {
        var list = ApparelController.Instance.GetModifierList();
        Tuple<string, Sprite> values = GetInfo(ApparelController.Instance.GetModifierFromIndex(list, index));

        Image image = gameObject.FindChildWithTag("Icon", true)?.GetComponent<Image>();
        image?.gameObject.SetActive(values.Item2);

        _component.Item1.Invoke(values.Item1);
        _component.Item2.Invoke(values.Item2);
    }
}
