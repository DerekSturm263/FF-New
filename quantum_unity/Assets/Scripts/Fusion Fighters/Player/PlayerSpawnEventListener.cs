using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawnEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame, EntityRef, int> _onSpawn;
    [SerializeField] private UnityEvent<QuantumGame, EntityRef, int> _onDespawn;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: e => _onSpawn.Invoke(e.Game, e.Player, e.Index));
    }
}
