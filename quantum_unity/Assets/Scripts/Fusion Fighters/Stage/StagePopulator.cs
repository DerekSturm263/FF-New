using Quantum.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StagePopulator : PopulateSerializable<Quantum.Stage, StageAssetAsset>
{
    public enum CheckmarkType
    {
        InList,
        Selected
    }

    [SerializeField] private CheckmarkType _type = CheckmarkType.InList;

    protected override string BuiltInFilePath() => "DB/Assets/Stage/Stages";
    protected override string CustomFilePath() => StageController.GetPath();

    protected override SerializableWrapper<Quantum.Stage> GetFromBuiltInAsset(StageAssetAsset asset)
    {
        var item = asset.Stage;

        return item;
    }

    protected override bool IsEquipped(SerializableWrapper<Quantum.Stage> item)
    {
        if (_type == CheckmarkType.InList)
            return ArrayHelper.All(RulesetController.Instance.CurrentRuleset.value.Stage.Stages).Any(stage => stage.Id == item.FileID);
        else
            return StageController.Instance.CurrentStage.value.Equals(item.value);
    }

    protected override bool IsNone(SerializableWrapper<Quantum.Stage> item) => false;

    protected override Dictionary<string, Predicate<SerializableWrapper<Quantum.Stage>>> GetAllFilterModes()
    {
        Dictionary<string, Predicate<SerializableWrapper<Quantum.Stage>>> tagGroups = new();

        foreach (var key in _itemsToButtons.Keys)
        {
            if (key.FilterTags is not null)
            {
                foreach (string tag in key.FilterTags)
                {
                    tagGroups.TryAdd(tag, (value) =>
                    {
                        if (value.FilterTags is not null)
                            return value.FilterTags.Contains(tag) && IsInList(value);

                        return IsInList(value);
                    });
                }
            }
        }

        Dictionary<string, Predicate<SerializableWrapper<Quantum.Stage>>> baseFilters = new();

        if (_loadType.HasFlag(CreationType.BuiltIn) && _loadType.HasFlag(CreationType.Custom))
        {
            baseFilters.Add("All", IsInList);
            baseFilters.Add("Custom", (value) => value.MadeByPlayer && IsInList(value));
            baseFilters.Add("Built-In", (value) => !value.MadeByPlayer && IsInList(value));
        }
        else
        {
            baseFilters.Add("All", IsInList);
        }

        return baseFilters.Concat(tagGroups).ToDictionary(item => item.Key, item => item.Value);
    }

    private bool IsInList(SerializableWrapper<Quantum.Stage> item) => ArrayHelper.All(RulesetController.Instance.CurrentRuleset.value.Stage.Stages).Contains(new() { Id = item.FileID });
}
