using Quantum;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTracker<T> : MonoBehaviour
{
    private EntityViewUpdater _entityView;

    protected Dictionary<EntityView, T> _playersToTs = new();

    private void Awake()
    {
        _entityView = FindFirstObjectByType<EntityViewUpdater>();
    }

    private void Update()
    {
        foreach (var kvp in _playersToTs)
            Action(kvp.Key, kvp.Value);
    }

    protected abstract void Action(EntityView player, T t);

    public void TrackPlayer(QuantumGame game, PlayerLink player)
    {
        EntityView entity = _entityView.GetView(player.Entity);
        _playersToTs.Add(entity, GetT(game, player));
    }

    public void UntrackPlayer(QuantumGame game, PlayerLink player)
    {
        EntityView entity = _entityView.GetView(player.Entity);
        CleanUp(_playersToTs[_entityView.GetView(player.Entity)]);

        _playersToTs.Remove(entity);
    }

    protected abstract T GetT(QuantumGame game, PlayerLink player);
    protected abstract void CleanUp(T t);
}
