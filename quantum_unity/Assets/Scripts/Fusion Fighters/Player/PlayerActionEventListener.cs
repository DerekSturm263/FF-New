using GameResources;
using Quantum;
using UnityEngine;

public class PlayerActionEventListener : MonoBehaviour
{
    [SerializeField] private VFX _jump;
    [SerializeField] private VFX _spawnDespawn;

    private EntityViewUpdater _entityViewUpdater;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: Spawn);
        QuantumEvent.Subscribe<EventOnPlayerDespawn>(listener: this, handler: Despawn);
        QuantumEvent.Subscribe<EventOnPlayerChangeDirection>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().ChangeDirection(e.Direction));
        QuantumEvent.Subscribe<EventOnPlayerJump>(listener: this, handler: Jump);
        QuantumEvent.Subscribe<EventOnHurtboxStateChange>(listener: this, handler: UpdateHurtboxSettings);
    }

    public void Spawn(EventOnPlayerSpawn e)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_spawnDespawn);

        if (effect)
            effect.transform.position = _entityViewUpdater.GetView(e.Ctx.Entity).transform.position;
    }

    public void Despawn(EventOnPlayerDespawn e)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_spawnDespawn);

        if (effect)
            effect.transform.position = _entityViewUpdater.GetView(e.Ctx.Entity).transform.position;
    }

    public void Jump(EventOnPlayerJump e)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_jump);

        if (effect)
            effect.transform.position = _entityViewUpdater.GetView(e.Player).transform.position;
    }

    public void UpdateHurtboxSettings(EventOnHurtboxStateChange e)
    {
        _entityViewUpdater.GetView(e.Owner).GetComponentInChildren<PlayerEventReceiver>().SetHurtboxState(e.Settings);
    }
}
