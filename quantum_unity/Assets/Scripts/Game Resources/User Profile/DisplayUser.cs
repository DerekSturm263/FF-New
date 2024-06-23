using UnityEngine;
using UnityEngine.UI;

public class DisplayUser : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _playerNum;
    [SerializeField] private Image _controllerIcon;
    [SerializeField] private TMPro.TMP_Text _playerName;

    public void SetUser(LocalPlayerInfo playerInfo)
    {
        _playerNum.SetText($"P{playerInfo.Index + 1}");
        //_controllerIcon.sprite = ;
        _playerName.SetText(playerInfo.Profile.Name);
    }
}
