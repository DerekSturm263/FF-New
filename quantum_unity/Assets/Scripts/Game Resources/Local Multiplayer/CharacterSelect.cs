using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private CustomMultiplayerUIInputSystemModule _inputModule;

    public void Bind(LocalPlayerInfo player)
    {
        _inputModule.BindControls(player);
    }
}
