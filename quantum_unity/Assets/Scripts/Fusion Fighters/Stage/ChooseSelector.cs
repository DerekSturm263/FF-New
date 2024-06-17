using Quantum;
using UnityEngine;

public class ChooseSelector : MonoBehaviour
{
    [SerializeField] private GameObject _stages;

    private CustomMultiplayerUIInputSystemModule _inputSystem;

    private unsafe void Start()
    {
        _inputSystem = GetComponent<CustomMultiplayerUIInputSystemModule>();

        int selector = QuantumRunner.Default.Game.Frames.Verified.Global->LastSelector;
        _stages.SetActive(selector == -1 || selector == _inputSystem.PlayerInfo.GlobalIndex);
    }
}
