using UnityEngine;

public class EmotePopulator : PopulateAsset<EmoteAsset>
{
    [SerializeField] private DisplayEmote.Direction _direction;

    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Emotes";

    protected override bool IsEquipped(EmoteAsset item)
    {
        return _direction switch
        {
            DisplayEmote.Direction.Up => BuildController.Instance.CurrentBuild.value.Cosmetics.Emotes.Up.Emote.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Down => BuildController.Instance.CurrentBuild.value.Cosmetics.Emotes.Down.Emote.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Left => BuildController.Instance.CurrentBuild.value.Cosmetics.Emotes.Left.Emote.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Right => BuildController.Instance.CurrentBuild.value.Cosmetics.Emotes.Right.Emote.Id == item.AssetObject.Guid,
            _ => default
        };
    }
}
