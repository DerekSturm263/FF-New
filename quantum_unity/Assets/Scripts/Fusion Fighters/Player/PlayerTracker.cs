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
                TrackPlayer(QuantumRunner.Default.Game, new() { Entity = stats.Entity, Name = stats.Component.Name, Index = stats.Component.Index });
                Debug.Log("Player tracked!");
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

    public void TrackPlayer(QuantumGame game, PlayerInfoCallbackContext ctx)
    {
        _playersToTs.TryAdd(ctx.Entity, GetT(game, ctx));
    }

    public void UntrackPlayer(QuantumGame game, PlayerInfoCallbackContext ctx)
    {
        if (_playersToTs.TryGetValue(ctx.Entity, out T t))
            CleanUp(t);

        _playersToTs.Remove(ctx.Entity);
    }

    protected abstract T GetT(QuantumGame game, PlayerInfoCallbackContext ctx);
    protected abstract void CleanUp(T t);
}
