using Quantum;
using UnityEngine;

public class CustomCallbacks : QuantumCallbacks
{
    [SerializeField] private RuntimePlayer _playerData;

    public override void OnGameStart(QuantumGame game)
    {
        // paused on Start means waiting for Snapshot
        if (game.Session.IsPaused)
            return;

        foreach (var localPlayer in game.GetLocalPlayers())
        {
            Debug.Log("CustomCallbacks - sending player: " + localPlayer);
            game.SendPlayerData(localPlayer, _playerData);
        }
    }

    public override void OnGameResync(QuantumGame game)
    {
        Debug.Log("Detected Resync. Verified tick: " + game.Frames.Verified.Number);
    }
}

