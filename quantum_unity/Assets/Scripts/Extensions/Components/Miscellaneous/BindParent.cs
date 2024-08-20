using UnityEngine;

public class BindParent : MonoBehaviour
{
    [SerializeField] private CustomMultiplayerUIInputSystemModule _inputModule;

    private LocalPlayerInfo _player;
    public LocalPlayerInfo Player => _player;

    public void Bind(LocalPlayerInfo player)
    {
        _player = player;
        _inputModule.BindControls(player);
    }
}
