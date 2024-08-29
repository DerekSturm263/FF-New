using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = BehaviorAsset;

public class DisplayBehavior : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override unsafe Type GetValue()
    {
        EntityRef bot = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, item => item.Type == FighterType.Bot);

        if (QuantumRunner.Default.Game.Frames.Verified.Unsafe.TryGetPointer(bot, out Quantum.CharacterController* characterController))
            return UnityDB.FindAsset<Type>(characterController->Behavior.Id);

        return null;
    }
}
