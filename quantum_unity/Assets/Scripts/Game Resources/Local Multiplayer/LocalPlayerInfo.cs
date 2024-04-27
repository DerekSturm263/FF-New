using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[System.Serializable]
public class LocalPlayerInfo
{
    private static readonly Dictionary<InputDevice, LocalPlayerInfo> _allPlayers = new();
    public static Dictionary<InputDevice, LocalPlayerInfo> AllPlayers => _allPlayers;

    private readonly InputDevice _device;
    public InputDevice Device => _device;

    private readonly InputUser _user;
    public InputUser User => _user;

    private LocalPlayerInfo(InputDevice device)
    {
        if (device is not null)
        {
            _device = device;
            _user = InputUser.PerformPairingWithDevice(device);
        }
    }

    ~LocalPlayerInfo()
    {
        if (_user.id != InputUser.InvalidId && _device is not null)
            _user.UnpairDevice(_device);
    }

    public static LocalPlayerInfo GetPlayer(InputDevice device)
    {
        if (_allPlayers.TryGetValue(device, out var player))
            return player;

        return null;
    }

    public static LocalPlayerInfo AddPlayer(InputDevice device)
    {
        LocalPlayerInfo player = new(device);
        _allPlayers.Add(device, player);

        Debug.Log($"Player has joined via {device?.displayName}");

        return player;
    }

    public static LocalPlayerInfo RemovePlayer(InputDevice device) => RemovePlayer(GetPlayer(device));

    public static void RemoveAllPlayers()
    {
        for (int i = 0; i < _allPlayers.Count; ++i)
        {
            RemovePlayer(_allPlayers.First().Value);
        }
    }

    private static LocalPlayerInfo RemovePlayer(LocalPlayerInfo player)
    {
        if (player is not null)
            _allPlayers.Remove(player._device);

        Debug.Log($"Player has left the game");
        return player;
    }
}
