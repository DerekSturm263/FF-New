using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class CustomMultiplayerUIInputSystemModule : MonoBehaviour
{
    private MultiplayerEventSystem _eventSystem;

    private LocalPlayerInfo _playerInfo;
    public LocalPlayerInfo PlayerInfo => _playerInfo;

    [SerializeField] private Selector _selectorAsset;

    private Selector _selector;
    public Selector Selector => _selector;

    private static readonly float _cooldownTime = 0.35f;
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

        if (_playerInfo.Controls.Menu.Submit.WasPerformedThisFrame())
            Submit();

        if (_playerInfo.Controls.Menu.Cancel.WasPerformedThisFrame())
            Cancel();

        Navigate(_playerInfo.Controls.Menu.Navigate.ReadValue<Vector2>());
    }

    private void Navigate(Vector2 dir)
    {
        Selectable selectable = _eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        if (!selectable)
            return;

        Selectable navigationTarget = null;
        bool canMove = false;

        Quantum.Direction direction = DirectionalHelper.GetEnumFromDirection(new() { X = FP.FromFloat_UNSAFE(dir.x), Y = FP.FromFloat_UNSAFE(dir.y) });
        if (direction != Quantum.Direction.Neutral)
        {
            if (direction == _lastDirection && _cooldown <= 0)
            {
                _cooldown = _cooldownTime;
                canMove = true;

                Debug.Log("Cooldown = 0.5");
            }
        }
        else
        {
            _cooldown = 0;

            Debug.Log("Cooldown = 0");
            canMove = true;
        }

        _lastDirection = direction;

        if (canMove)
        {
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

            if (!navigationTarget || !navigationTarget.transform.IsChildOf(transform.GetChild(0)))
                return;

            Debug.Log("Select");

            _eventSystem.SetSelectedGameObject(navigationTarget.gameObject);
            _selector.ChildToSelected(_eventSystem.currentSelectedGameObject);
        }
    }

    private void Submit()
    {
        if (!_eventSystem.currentSelectedGameObject)
            return;

        PlayerEventData data = new(_eventSystem) { PlayerIndex = _playerInfo.Index };
        ExecuteEvents.Execute(_eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

        if (_eventSystem.currentSelectedGameObject.TryGetComponent(out MultiplayerButton button))
        {
            button.onClick.Invoke(_playerInfo.Index);
        }
    }

    private void Cancel()
    {
        PlayerEventData data = new(_eventSystem) { PlayerIndex = _playerInfo.Index };
        ExecuteEvents.Execute(_eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
    }

    public void BindControls(LocalPlayerInfo player)
    {
        if (player is null || player.User.id == InputUser.InvalidId)
            return;

        _playerInfo = player;

        _selector = Instantiate(_selectorAsset, transform);
        _selector.Initialize(_playerInfo.Index);

        if (gameObject.activeInHierarchy)
            _playerInfo?.Controls?.Menu.Enable();
        else
            _playerInfo?.Controls?.Menu.Disable();
    }

    private void OnEnable() => _playerInfo?.Controls?.Menu.Enable();
    private void OnDisable() => _playerInfo?.Controls?.Menu.Disable();
}
