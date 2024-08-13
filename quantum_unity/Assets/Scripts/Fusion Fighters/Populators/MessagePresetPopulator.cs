using UnityEngine;

public class MessagePresetPopulator : PopulateAsset<MessagePresetAsset>
{
    [SerializeField] private DisplayEmote.Direction _direction;

    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Message Presets";

    protected override string Name(MessagePresetAsset item) => !item.name.Equals("None") ? item.Description : "None";

    protected override bool IsEquipped(MessagePresetAsset item)
    {
        return _direction switch
        {
            DisplayEmote.Direction.Up => BuildController.Instance.CurrentBuild.value.Emotes.Up.Message.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Down => BuildController.Instance.CurrentBuild.value.Emotes.Down.Message.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Left => BuildController.Instance.CurrentBuild.value.Emotes.Left.Message.Id == item.AssetObject.Guid,
            DisplayEmote.Direction.Right => BuildController.Instance.CurrentBuild.value.Emotes.Right.Message.Id == item.AssetObject.Guid,
            _ => default
        };
    }
}
