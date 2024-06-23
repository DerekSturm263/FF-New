using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using Quantum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserProfileController : Controller<UserProfileController>
{
    [SerializeField] private GameObject _asset;
    private GameObject _current;

    private LocalPlayerInfo _player;
    public void SetPlayer(LocalPlayerInfo player) => _player = player;

    [SerializeField] private PopulateBase _populator;

    [NonSerialized] private bool _isSpawningPlayer;
    public bool IsSpawningPlayer => _isSpawningPlayer;
    
    private GameObject _oldSelected;
    private List<(Selectable, bool)> _allSelectables;

    private IEnumerable<Extensions.Components.Input.InputEvent> _inputEvents;
    private Dictionary<Extensions.Components.Input.InputEvent, bool> _wasEnabled;

    public override void Initialize()
    {
        base.Initialize();

        _isSpawningPlayer = false;
    }

    public void Spawn()
    {
        RememberOldSelected();

        _allSelectables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select<Selectable, (Selectable, bool)>(item => new(item, item.interactable)).ToList();
        foreach (var selectable in _allSelectables)
        {
            selectable.Item1.interactable = false;
        }

        _inputEvents = FindObjectsByType<Extensions.Components.Input.InputEvent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(item => item.gameObject != gameObject);
        _wasEnabled = new();

        foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
        {
            _wasEnabled.Add(inputEvent, inputEvent.enabled);
            inputEvent.enabled = false;
        }

        _current = Instantiate(_asset, GameObject.FindWithTag("Popup Canvas").transform);
        _isSpawningPlayer = true;
    }

    public void RememberOldSelected()
    {
        if (EventSystem.current)
            _oldSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void SetOldSelected()
    {
        if (EventSystem.current && _oldSelected)
            EventSystem.current.SetSelectedGameObject(_oldSelected);
    }

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Profiles";

    public SerializableWrapper<UserProfile> New()
    {
        UserProfile profile = new();
        return new(profile, _name, "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
    }

    public void Save(SerializableWrapper<UserProfile> profile)
    {
        Serializer.Save(profile, profile.Guid, GetPath());
    }

    public void SaveNew()
    {
        UserProfile profile = new();
        SerializableWrapper<UserProfile> serialized = new(profile, _name, "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);

        Serializer.Save(serialized, serialized.Guid, GetPath());
    }

    private SerializableWrapper<UserProfile> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<UserProfile> profile) => _currentlySelected = profile;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(UserProfilePopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public void SetName(string name)
    {
        _currentlySelected.SetName(name);
    }

    private string _name;
    public void SetNameUnfinished(string name) => _name = name;

    public void SetOnPlayer(SerializableWrapper<UserProfile> profile)
    {
        _player.SetProfile(profile);
        FindFirstObjectByType<DisplayUsers>()?.UpdateDisplay();
    }

    private Action _action;

    public void DeferEvents(Action action)
    {
        _action = action;
    }

    public void ExecuteDeferredEvents()
    {
        _action.Invoke();
    }

    public void Close()
    {
        foreach (var selectable in _allSelectables)
        {
            selectable.Item1.interactable = selectable.Item2;
        }

        SetOldSelected();

        foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
        {
            inputEvent.enabled = _wasEnabled[inputEvent];
        }

        Destroy(_current);
        _current = null;

        _isSpawningPlayer = false;
    }

    public void Cancel()
    {
        PlayerJoinController.Instance.RemovePlayer(_player);
    }
}
