using Quantum;
using UnityEngine;

public class ChooseSelector : MonoBehaviour
{
    [SerializeField] private GameObject _stages;

    private CustomMultiplayerUIInputSystemModule _inputSystem;

    private unsafe void Start()
    {
        _inputSystem = GetComponent<CustomMultiplayerUIInputSystemModule>();
    }
}
