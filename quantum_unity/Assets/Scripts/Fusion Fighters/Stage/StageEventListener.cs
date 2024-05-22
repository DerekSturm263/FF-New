using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class StageEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<Stage> _onStageSwitch;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnStageSelect>(listener: this, handler: e => _onStageSwitch.Invoke(e.Stage));
    }
}
