using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class PlayerReadyEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame> _onAllPlayersReady;
    [SerializeField] private UnityEvent<QuantumGame> _onAllPlayersCancel;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnAllPlayersReady>(listener: this, handler: e => _onAllPlayersReady.Invoke(e.Game));
        QuantumEvent.Subscribe<EventOnAllPlayersCancel>(listener: this, handler: e => _onAllPlayersCancel.Invoke(e.Game));
    }
}
