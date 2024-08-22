using Extensions.Components.Input;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUser : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _playerNum;
    [SerializeField] private Image _controllerIcon;
    [SerializeField] private TMPro.TMP_Text _playerName;
    [SerializeField] private GameObject _joinPrompt;

    public void SetUser(LocalPlayerInfo playerInfo)
    {
        _playerNum.SetText($"P{playerInfo.Index.GlobalNoBots + 1}");
        _playerName.SetText(playerInfo.Profile.Name);

        _controllerIcon.sprite = InputMapperController.Instance.GetIcon(playerInfo.Device.displayName);
    }

    public void SetJoinPromptVisibility(bool canJoin)
    {
        _joinPrompt.SetActive(canJoin);
    }
}
