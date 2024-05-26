using Quantum.Types;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class UserUILink : MonoBehaviour
{
    private MultiplayerEventSystem _eventSystem;
    private Controls _controls;

    private void Awake()
    {
        _eventSystem = GetComponent<MultiplayerEventSystem>();

        _controls = new();

        _controls.Menu.Navigate.performed += Navigate;
        _controls.Menu.Submit.performed += Submit;
        _controls.Menu.Cancel.performed += Cancel;
    }

    private void Navigate(InputAction.CallbackContext ctx)
    {
        GameObject selectedObj =_eventSystem.currentSelectedGameObject;
        if (!selectedObj)
            return;

        Selectable selected = selectedObj.GetComponent<Selectable>();

        Quantum.Direction direction = DirectionalAssetHelper.GetEnumFromDirection(ctx.ReadValue<Vector2>().ToFPVector2());
        switch (direction)
        {
            case Quantum.Direction.Up:
                Selectable selectable = selected.FindSelectableOnUp();
                if (selectable)
                    _eventSystem.SetSelectedGameObject(selectable.gameObject);

                break;

            case Quantum.Direction.Down:
                Selectable selectable2 = selected.FindSelectableOnDown();
                if (selectable2)
                    _eventSystem.SetSelectedGameObject(selectable2.gameObject);

                break;

            case Quantum.Direction.Left:
                Selectable selectable3 = selected.FindSelectableOnLeft();
                if (selectable3)
                    _eventSystem.SetSelectedGameObject(selectable3.gameObject);

                break;

            case Quantum.Direction.Right:
                Selectable selectable4 = selected.FindSelectableOnRight();
                if (selectable4)
                    _eventSystem.SetSelectedGameObject(selectable4.gameObject);

                break;
        }
    }

    private void Submit(InputAction.CallbackContext ctx)
    {
        Selectable selected = _eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        selected.OnSelect(new(_eventSystem));
    }

    private void Cancel(InputAction.CallbackContext ctx)
    {

    }

    public void BindControls(LocalPlayerInfo player)
    {
        if (player is null || player.User.id == InputUser.InvalidId)
            return;

        player.User.AssociateActionsWithUser(_controls);

        InputControlScheme? scheme = InputControlScheme.FindControlSchemeForDevice(player.Device, _controls.controlSchemes);
        if (scheme.HasValue)
        {
            player.User.ActivateControlScheme(scheme.Value);
        }
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}
