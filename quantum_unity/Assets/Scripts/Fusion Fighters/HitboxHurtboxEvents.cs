using Quantum;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitboxHurtboxEvents : MonoBehaviour
{
    private EntityViewUpdater _viewUpdater;

    private void Awake()
    {
        _viewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnHitboxHurtboxCollision>(listener: this, handler: HandleHitboxHurtboxCollision);
        QuantumEvent.Subscribe<EventOnHitboxSpawnDespawn>(listener: this, handler: HandleHitboxSpawnDespawn);
    }

    private void HandleHitboxHurtboxCollision(EventOnHitboxHurtboxCollision e)
    {
        if (PlayerJoinController.Instance.TryGetPlayer(e.DefenderIndex, out LocalPlayerInfo player))
        {
            PlayerJoinController.Instance.Rumble(player, player.Profile.value.HapticStrength * e.Settings.TargetShakeIntensity.AsFloat * 0.1f, 0.3f);
        }
    }

    private void HandleHitboxSpawnDespawn(EventOnHitboxSpawnDespawn e)
    {
        _viewUpdater.GetView(e.Owner).gameObject.GetComponentInChildren<TrailRenderer>().emitting = e.IsEnabled;
    }
}
