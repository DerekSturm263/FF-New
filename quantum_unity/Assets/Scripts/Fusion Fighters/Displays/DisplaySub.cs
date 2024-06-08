using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Sub>;

public class DisplaySub : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.Value.SerializableData.Name}</size></font>\n\n{item.Value.SerializableData.Description}", item.Icon);
    }

    protected override Type GetValue() => default;
}
