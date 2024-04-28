using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class ReadyPlayerController : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerReady;
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerCancel;
    [SerializeField] private UnityEvent<QuantumGame> _onAllPlayersReady;
    [SerializeField] private UnityEvent<QuantumGame> _onAllPlayersCancel;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnPlayerReady>(listener: this, handler: e => _onPlayerReady.Invoke(e.Game, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerCancel>(listener: this, handler: e => _onPlayerCancel.Invoke(e.Game, e.Player));
        QuantumEvent.Subscribe<EventOnAllPlayersReady>(listener: this, handler: e => _onAllPlayersReady.Invoke(e.Game));
        QuantumEvent.Subscribe<EventOnAllPlayersCancel>(listener: this, handler: e => _onAllPlayersCancel.Invoke(e.Game));
    }
}
