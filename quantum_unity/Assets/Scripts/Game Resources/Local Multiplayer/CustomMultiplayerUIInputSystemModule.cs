using Quantum.Types;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class CustomMultiplayerUIInputSystemModule : MonoBehaviour
{
    private Controls _controls;

    private MultiplayerEventSystem _eventSystem;
    private LocalPlayerInfo _playerInfo;

    [SerializeField] private Selector _selectorAsset;

    private Selector _selector;
    public Selector Selector => _selector;

    [SerializeField] private float _cooldownTime;
    private float _cooldown = 0;
    private Quantum.Direction _lastDirection = Quantum.Direction.Neutral;

    private void Awake()
    {
        _eventSystem = GetComponent<MultiplayerEventSystem>();
    }

    private void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.deltaTime;
    }

    public void MapControls()
    {
        _controls.Menu.Navigate.performed += Navigate;
        _controls.Menu.Submit.performed += Submit;
        _controls.Menu.Cancel.performed += Cancel;
    }

    private void Navigate(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != _playerInfo.Device)
            return;

        Selectable selectable = _eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        if (!selectable)
            return;

        Selectable navigationTarget = null;

        Quantum.Direction direction = DirectionalAssetHelper.GetEnumFromDirection(ctx.ReadValue<Vector2>().ToFPVector2());
        if (direction == _lastDirection)
            if (_cooldown == 0)
                _cooldown = _cooldownTime;
        else
            _cooldown = 0;

        if (_cooldown > 0)
            return;
        
        switch (direction)
        {
            case Quantum.Direction.Right:
                navigationTarget = selectable.FindSelectableOnRight();
                break;

            case Quantum.Direction.Up:
                navigationTarget = selectable.FindSelectableOnUp();
                break;

            case Quantum.Direction.Left:
                navigationTarget = selectable.FindSelectableOnLeft();
                break;

            case Quantum.Direction.Down:
                navigationTarget = selectable.FindSelectableOnDown();
                break;
        }

        _lastDirection = direction;

        if (!navigationTarget || !navigationTarget.transform.IsChildOf(transform.GetChild(0)))
            return;

        _eventSystem.SetSelectedGameObject(navigationTarget.gameObject);
        _selector.ChildToSelected(_eventSystem.currentSelectedGameObject);
    }

    private void Submit(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != _playerInfo.Device)
            return;

        if (!_eventSystem.currentSelectedGameObject)
            return;

        PlayerEventData data = new(_eventSystem) { PlayerNum = _playerInfo.User.index };
        ExecuteEvents.Execute(_eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

        if (_eventSystem.currentSelectedGameObject.TryGetComponent(out MultiplayerButton button))
        {
            button.onClick.Invoke(_playerInfo.User.index);
        }
    }

    private void Cancel(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != _playerInfo.Device)
            return;

        PlayerEventData data = new(_eventSystem) { PlayerNum = _playerInfo.User.index };
        ExecuteEvents.Execute(_eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
    }

    public void BindControls(LocalPlayerInfo player)
    {
        if (player is null || player.User.id == InputUser.InvalidId)
            return;

        _controls = new();
        _selector = Instantiate(_selectorAsset, transform);

        player.User.AssociateActionsWithUser(_controls);
        InputControlScheme? scheme = InputControlScheme.FindControlSchemeForDevice(player.Device, _controls.controlSchemes);
        if (scheme.HasValue)
        {
            player.User.ActivateControlScheme(scheme.Value);
        }

        _playerInfo = player;
        _selector.Initialize(_playerInfo.User.index);

        MapControls();

        if (gameObject.activeInHierarchy)
            _controls.Enable();
    }

    private void OnEnable() => _controls?.Enable();
    private void OnDisable() => _controls?.Disable();
}
