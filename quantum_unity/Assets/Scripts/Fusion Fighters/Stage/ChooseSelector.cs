using Quantum;
using UnityEngine;

public class ChooseSelector : MonoBehaviour
{
    [SerializeField] private GameObject _stages;

    private CustomMultiplayerUIInputSystemModule _inputSystem;

    private unsafe void Start()
    {
        _inputSystem = GetComponent<CustomMultiplayerUIInputSystemModule>();

        FighterIndex selector = QuantumRunner.Default.Game.Frames.Verified.Global->LastSelector;

        _stages.SetActive(selector.Equals(FighterIndex.Invalid) || selector.Equals(_inputSystem.PlayerInfo.Index));
    }
}
