using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public class PlayerJoinEventListener :  MonoBehaviour
{
    [SerializeField] private UnityEvent<LocalPlayerInfo> _onPlayerJoin;
    public void InvokeOnPlayerJoin(LocalPlayerInfo player)
    {
        if (!_isEnabled)
            return;

        _onPlayerJoin?.Invoke(player);
    }

    [SerializeField] private UnityEvent<LocalPlayerInfo> _onPlayerLeave;
    public void InvokeOnPlayerLeave(LocalPlayerInfo player)
    {
        if (!_isEnabled)
            return;

        _onPlayerLeave?.Invoke(player);
    }

    [SerializeField] private RectTransform _characterSelectParent;
    [SerializeField] private RectTransform _stageSelectParent;

    [SerializeField] private BindParent _characterSelectMenu;
    [SerializeField] private BindParent _stageSelectMenu;

    private Dictionary<LocalPlayerInfo, List<BindParent>> _menus = new();

    [SerializeField] private bool _isEnabled;
    public bool IsEnabled => _isEnabled;
    public void SetEnabled(bool isEnabled) => _isEnabled = isEnabled;

    public void Invoke()
    {
        _isEnabled = true;

        foreach (var player in PlayerJoinController.Instance.AllPlayers.Values)
            InvokeOnPlayerJoin(player);
    }

    public void BindSelectScreens(LocalPlayerInfo player)
    {
        BindParent css = Instantiate(_characterSelectMenu, _characterSelectParent.transform);
        css.Bind(player);

        if (css.gameObject.activeInHierarchy)
            css.GetComponentInChildren<SelectAuto>().SetSelectedItem();

        BindParent ss = Instantiate(_stageSelectMenu, _stageSelectParent.transform);
        ss.Bind(player);

        if (ss.gameObject.activeInHierarchy)
            ss.GetComponentInChildren<SelectAuto>().SetSelectedItem();

        _menus.Add(player, new() { css, ss });
     
        Debug.Log($"Created HUD controls for local player {player.LocalIndex}");
    }

    public void UnbindSelectScreens(LocalPlayerInfo player)
    {
        foreach (var item in _menus[player])
        {
            Destroy(item);
        }
    }
}