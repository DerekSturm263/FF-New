using Extensions.Components.UI;
using Extensions.Types;
using Quantum.Types;
using System.Linq;
using System.Text;
using UnityEngine;

using Type = Quantum.ArrayStages;

public class DisplayStages : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        StringBuilder stages = new();

        if (ArrayHelper.All(item).Count(item => item.Id.IsValid) > 0)
        {
            foreach (var stage in ArrayHelper.All(item))
            {
                var stageAsset = UnityDB.FindAsset<StageAssetAsset>(stage.Id);
                if (stageAsset)
                {
                    stages.Append(stageAsset.name + ", ");
                }
            }

            stages.Remove(stages.Length - 2, 2);
        }
        else
        {
            stages.Append("None");
        }

        return new(string.Format(_format, stages), null);
    }

    protected override Type GetValue() => RulesetController.Instance.CurrentRuleset.value.Stage.Stages;
}
