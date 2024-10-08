using GameResources;
using Quantum;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionEventListener : MonoBehaviour
{
    [SerializeField] private VFX _jump;
    [SerializeField] private VFX _spawnDespawn;

    [SerializeField] private GameObject _knockbackTrail;
    [SerializeField] private Vector3 _trailOffset;

    private Dictionary<EntityRef, ParticleSystem> _playersToTrails = new();

    private EntityViewUpdater _entityViewUpdater;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: Spawn);
        QuantumEvent.Subscribe<EventOnPlayerDespawn>(listener: this, handler: Despawn);
        QuantumEvent.Subscribe<EventOnPlayerChangeDirection>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().ChangeDirection(e.Direction));
        QuantumEvent.Subscribe<EventOnPlayerJump>(listener: this, handler: Jump);
        QuantumEvent.Subscribe<EventOnHurtboxStateChange>(listener: this, handler: UpdateHurtboxSettings);
        QuantumEvent.Subscribe<EventOnPlayerRespawn>(listener: this, handler: Respawn);
        QuantumEvent.Subscribe<EventOnPlayerHoldAnimation>(listener: this, handler: UpdateHurtboxToHold);
        QuantumEvent.Subscribe<EventOnSwitchWeapon>(listener: this, handler: SwitchWeapon);
        QuantumEvent.Subscribe<EventOnPlayerKnockback>(listener: this, handler: Knockback);
    }

    public void Spawn(EventOnPlayerSpawn e)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_spawnDespawn);

        if (effect)
            effect.transform.position = _entityViewUpdater.GetView(e.Ctx.Entity).transform.position;
    }

    public void Respawn(EventOnPlayerRespawn e)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_spawnDespawn);

        if (effect)
            effect.transform.position = e.Ctx.Position.ToUnityVector2();
    }

    public void Despawn(EventOnPlayerDespawn e)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_spawnDespawn);

        if (effect)
            effect.transform.position = e.Ctx.Position.ToUnityVector3();
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

    public void UpdateHurtboxToHold(EventOnPlayerHoldAnimation e)
    {
        if (e.IsHolding)
            _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetHurtboxState(new() { CanBeDamaged = true, CanBeInterrupted = false, CanBeKnockedBack = false, DamageToBreak = 0 });
        else
            _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetHurtboxState(new() { CanBeDamaged = true, CanBeInterrupted = true, CanBeKnockedBack = true, DamageToBreak = 0 });
    }

    public void SwitchWeapon(EventOnSwitchWeapon e)
    {
        _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SwitchWeapons(e.State);
    }

    public void Knockback(EventOnPlayerKnockback e)
    {
        if (e.IsKnockingBack)
        {
            if (_playersToTrails.ContainsKey(e.Player))
                return;

            GameObject trail = Instantiate(_knockbackTrail, Vector3.zero, Quaternion.identity);
            _playersToTrails.Add(e.Player, trail.GetComponent<ParticleSystem>());

            trail.transform.position = _entityViewUpdater.GetView(e.Player).transform.position + _trailOffset;

            Follow follow = trail.AddComponent<Follow>();
            follow.SetTarget(_entityViewUpdater.GetView(e.Player).transform, _trailOffset);
        }
        else
        {
            if (_playersToTrails.TryGetValue(e.Player, out ParticleSystem ps))
            {
                ps.Stop();
                _playersToTrails.Remove(e.Player);
            }
        }
    }
}
