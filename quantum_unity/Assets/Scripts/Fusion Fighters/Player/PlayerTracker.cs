using Quantum;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTracker<T> : MonoBehaviour
{
    private EntityViewUpdater _entityView;

    protected Dictionary<EntityView, T> _playersToTs;

    private void Awake()
    {
        _entityView = FindFirstObjectByType<EntityViewUpdater>();
        _playersToTs = new();
    }

    private void Update()
    {
        foreach (var kvp in _playersToTs)
            Action(kvp.Key, kvp.Value);
    }

    protected abstract void Action(EntityView player, T t);

    public void TrackPlayer(QuantumGame game, EntityRef player, int index)
    {
        EntityView entity = _entityView.GetView(player);
        _playersToTs.TryAdd(entity, GetT(game, player, index));
    }

    public void UntrackPlayer(QuantumGame game, EntityRef player, int index)
    {
        EntityView entity = _entityView.GetView(player);
        CleanUp(_playersToTs[_entityView.GetView(player)]);

        _playersToTs.Remove(entity);
    }

    protected abstract T GetT(QuantumGame game, EntityRef player, int index);
    protected abstract void CleanUp(T t);
}
