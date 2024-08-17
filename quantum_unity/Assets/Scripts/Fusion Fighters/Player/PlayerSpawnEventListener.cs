using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawnEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame, PlayerInfoCallbackContext> _onSpawn;
    [SerializeField] private UnityEvent<QuantumGame, PlayerInfoCallbackContext> _onDespawn;

    [SerializeField] private RenderTexture[] _playerIcons;
    public RenderTexture[] PlayerIcons => _playerIcons;

    private EntityViewUpdater _entityViewUpdater;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: e =>
        {
            _entityViewUpdater.GetView(e.Ctx.Entity).GetComponentInChildren<Camera>().targetTexture = _playerIcons[e.Ctx.Index.Global];
            _onSpawn.Invoke(e.Game, e.Ctx);
        });
        QuantumEvent.Subscribe<EventOnPlayerDespawn>(listener: this, handler: e => _onDespawn.Invoke(e.Game, e.Ctx));
    }
}
