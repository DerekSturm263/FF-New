using Quantum;
using UnityEngine;

public class AlleywayLightsController : PlayerTracker<Transform>
{
    [SerializeField] private GameObject _spotlight;
    [SerializeField] private float _speed;

    [SerializeField] private Transform[] _lights;

    protected override void Action(EntityView player, Transform t)
    {
        t.forward = Vector3.Lerp(t.transform.forward, (player.transform.position - t.transform.position).normalized, Time.deltaTime * _speed);
    }

    protected override Transform GetT(QuantumGame game, PlayerLink player)
    {
        return Instantiate(_spotlight, _lights[player.Player._index - 1]).transform;
    }

    protected override void CleanUp(Transform t)
    {
        Destroy(t.gameObject);
    }
}
