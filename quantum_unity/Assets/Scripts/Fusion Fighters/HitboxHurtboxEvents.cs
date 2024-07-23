using Quantum;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitboxHurtboxEvents : MonoBehaviour
{
    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnHitboxHurtboxCollision>(listener: this, handler: HandleHitboxHurtboxCollision);
    }

    private void HandleHitboxHurtboxCollision(EventOnHitboxHurtboxCollision e)
    {
        LocalPlayerInfo player = PlayerJoinController.Instance.GetPlayer(e.DefenderIndex.Local);
        StartCoroutine(Rumble(player.Device as Gamepad, 0.5f, 0.5f));
    }

    IEnumerator Rumble(Gamepad gamepad, float frequency, float time)
    {
        gamepad.SetMotorSpeeds(frequency, frequency);

        gamepad.ResumeHaptics();
        yield return new WaitForSeconds(time);
        gamepad.PauseHaptics();
    }
}
