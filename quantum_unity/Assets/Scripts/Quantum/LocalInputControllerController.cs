using UnityEngine;

public class LocalInputControllerController : MonoBehaviour
{
    public void AddController(GamepadJoinCallbackContext ctx)
    {
        LocalInputController localController = gameObject.AddComponent<LocalInputController>();
        localController.BindControls(ctx.PlayerInfo);
    }
}
