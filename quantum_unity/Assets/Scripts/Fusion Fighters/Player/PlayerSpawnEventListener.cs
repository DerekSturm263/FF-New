using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawnEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame, EntityRef, QString32, int> _onSpawn;
    [SerializeField] private UnityEvent<QuantumGame, EntityRef, QString32, int> _onDespawn;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: e => _onSpawn.Invoke(e.Game, e.Player, e.Name, e.Index.Global));
        QuantumEvent.Subscribe<EventOnPlayerDespawn>(listener: this, handler: e => _onDespawn.Invoke(e.Game, e.Player, e.Name, e.Index.Global));
    }
}
