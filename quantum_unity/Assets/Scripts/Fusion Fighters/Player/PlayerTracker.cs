using Quantum;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTracker<T> : MonoBehaviour
{
    protected Dictionary<EntityView, T> _playersToTs;

    private void Awake()
    {
        _playersToTs = new();
    }

    private void Update()
    {
        foreach (var kvp in _playersToTs)
            Action(kvp.Key, kvp.Value);
    }

    protected abstract void Action(EntityView player, T t);

    public void TrackPlayer(QuantumGame game, EntityRef player, QString32 name, int index)
    {
        EntityView entity = FindFirstObjectByType<EntityViewUpdater>().GetView(player);
        _playersToTs.TryAdd(entity, GetT(game, player, name, index));
    }

    public void UntrackPlayer(QuantumGame game, EntityRef player, QString32 name, int index)
    {
        EntityView entity = FindFirstObjectByType<EntityViewUpdater>().GetView(player);
        CleanUp(_playersToTs[FindFirstObjectByType<EntityViewUpdater>().GetView(player)]);

        _playersToTs.Remove(entity);
    }

    protected abstract T GetT(QuantumGame game, EntityRef player, QString32 name, int index);
    protected abstract void CleanUp(T t);
}
