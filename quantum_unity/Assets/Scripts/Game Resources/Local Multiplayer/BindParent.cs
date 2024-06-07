using UnityEngine;

public class BindParent : MonoBehaviour
{
    [SerializeField] private CustomMultiplayerUIInputSystemModule _inputModule;

    public void Bind(LocalPlayerInfo player)
    {
        _inputModule.BindControls(player);
    }
}
