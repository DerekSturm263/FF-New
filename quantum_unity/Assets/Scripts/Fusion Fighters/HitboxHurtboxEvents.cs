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
        LocalPlayerInfo player = PlayerJoinController.Instance.GetPlayer(e.DefenderIndex.Local);
        StartCoroutine(Rumble(player.Device as Gamepad, player.Profile.Value.HapticStrength * e.Settings.TargetShakeIntensity.AsFloat * 0.1f, 0.3f));
    }

    private void HandleHitboxSpawnDespawn(EventOnHitboxSpawnDespawn e)
    {
        _viewUpdater.GetView(e.Owner).gameObject.GetComponentInChildren<TrailRenderer>().emitting = e.IsEnabled;
    }

    IEnumerator Rumble(Gamepad gamepad, float frequency, float time)
    {
        gamepad.SetMotorSpeeds(frequency, frequency);

        gamepad.ResumeHaptics();
        yield return new WaitForSeconds(time);
        gamepad.PauseHaptics();
    }
}
