using Quantum;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTracker<T> : MonoBehaviour
{
    protected EntityViewUpdater _viewUpdater;
    protected Dictionary<EntityRef, T> _playersToTs;

    protected virtual void Awake()
    {
        _viewUpdater = FindAnyObjectByType<EntityViewUpdater>();
        _playersToTs = new();

        if (QuantumRunner.Default)
        {
            foreach (var stats in QuantumRunner.Default.Game.Frames.Verified.GetComponentIterator<PlayerStats>())
            {
                TrackPlayer(QuantumRunner.Default.Game, stats.Entity, stats.Component.Name, stats.Component.Index);
            }
        }
    }

    private void Update()
    {
        foreach (var kvp in _playersToTs)
        {
            if (_viewUpdater)
            {
                EntityView view = _viewUpdater.GetView(kvp.Key);
                
                if (view)
                {
                    Action(view.gameObject, kvp.Value);
                }
            }
        }
    }

    protected abstract void Action(GameObject player, T t);

    public void TrackPlayer(QuantumGame game, EntityRef player, QString32 name, FighterIndex index)
    {
        _playersToTs.TryAdd(player, GetT(game, player, name, index));
    }

    public void UntrackPlayer(QuantumGame game, EntityRef player, QString32 name, FighterIndex index)
    {
        if (_playersToTs.TryGetValue(player, out T t))
            CleanUp(t);

        _playersToTs.Remove(player);
    }

    protected abstract T GetT(QuantumGame game, EntityRef player, QString32 name, FighterIndex index);
    protected abstract void CleanUp(T t);
}
