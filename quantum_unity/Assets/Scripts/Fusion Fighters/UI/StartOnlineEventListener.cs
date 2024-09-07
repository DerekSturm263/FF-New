using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class StartOnlineEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame> _onStart;
    public void Invoke(QuantumGame game) => _onStart?.Invoke(game);

    public void Defer() => DeferEvents.Defer(Invoke);
}
