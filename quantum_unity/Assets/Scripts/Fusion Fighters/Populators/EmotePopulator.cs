using UnityEngine;

public class EmotePopulator : PopulateAsset<EmoteAsset>
{
    [SerializeField] private DisplayEmote.Direction _direction;

    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Emotes";

    protected override bool DoSpawn(EmoteAsset item) => item.IncludeInLists && InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(EmoteAsset item)
    {
        return _direction switch
        {
            DisplayEmote.Direction.Up => BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Up.Emote.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Down => BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Down.Emote.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Left => BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Left.Emote.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Right => BuildController.Instance.CurrentlySelected.value.Cosmetics.Emotes.Right.Emote.Id == item.AssetObject.Guid,
            _ => default
        };
    }
}
