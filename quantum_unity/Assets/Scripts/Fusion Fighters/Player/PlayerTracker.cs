using Quantum;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTracker<T> : MonoBehaviour
{
    protected EntityViewUpdater _viewUpdater;
    protected Dictionary<EntityRef, T> _playersToTs;

    private void Awake()
    {
        _viewUpdater = FindAnyObjectByType<EntityViewUpdater>();
        _playersToTs = new();
    }

    private void Update()
    {
        foreach (var kvp in _playersToTs)
            Action(_viewUpdater.GetView(kvp.Key).gameObject, kvp.Value);
    }

    protected abstract void Action(GameObject player, T t);

    public void TrackPlayer(QuantumGame game, EntityRef player, QString32 name, int index)
    {
        _playersToTs.TryAdd(player, GetT(game, player, name, index));
    }

    public void UntrackPlayer(QuantumGame game, EntityRef player, QString32 name, int index)
    {
        if (_playersToTs.TryGetValue(player, out T t))
            CleanUp(t);

        _playersToTs.Remove(player);
    }

    protected abstract T GetT(QuantumGame game, EntityRef player, QString32 name, int index);
    protected abstract void CleanUp(T t);
}
