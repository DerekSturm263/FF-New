using Quantum;
using UnityEngine;

public class AlleywayLightsController : PlayerTracker<Transform>
{
    [SerializeField] private GameObject _spotlight;
    [SerializeField] private float _speed;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _followDistance;

    [SerializeField] private Transform[] _lights;

    protected override void Action(GameObject player, Transform t)
    {
        t.forward = Vector3.Lerp(t.transform.forward, (player.transform.position - t.transform.position).normalized, Time.deltaTime * _speed);
        t.position = Vector3.Lerp(t.position, player.transform.position - t.forward * _followDistance, Time.deltaTime * _moveSpeed);
    }

    protected override Transform GetT(QuantumGame game, PlayerInfoCallbackContext ctx)
    {
        return Instantiate(_spotlight, _lights[ctx.Index.Global]).transform;
    }

    protected override void CleanUp(Transform t)
    {
        Destroy(t.gameObject);
    }
}
