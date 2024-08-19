using Quantum;
using UnityEngine;

public class PlayerBuildEventListener : MonoBehaviour
{
    [SerializeField] private AssetRefFFAvatar _female, _male;

    private EntityViewUpdater _entityViewUpdater;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnPlayerSetAvatar>(listener: this, handler: e => SetAvatar(_entityViewUpdater.GetView(e.Player).gameObject, e.Old, e.New, e.Player));

        QuantumEvent.Subscribe<EventOnPlayerSetMainWeapon>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetMainWeapon(e.New, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerSetAltWeapon>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetAltWeapon(e.New, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerSetSub>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetSub(e.New, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerSetHeadgear>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetHeadgear(e.New, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerSetClothing>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetClothing(e.New, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerSetLegwear>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetLegwear(e.New, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerSetEyes>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetEyes(e.New, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerSetHair>(listener: this, handler: e => _entityViewUpdater.GetView(e.Player).GetComponentInChildren<PlayerEventReceiver>().SetHair(e.New, e.Player));
    }

    private void SetAvatar(GameObject player, AvatarColorBinding oldAvatar, AvatarColorBinding newAvatar, EntityRef entity)
    {
        PlayerEventReceiver[] eventListeners = player.GetComponentsInChildren<PlayerEventReceiver>(true);

        PlayerEventReceiver newAvatarObj = newAvatar.Avatar == _female ? eventListeners[0] : eventListeners[1];
        PlayerEventReceiver oldAvatarObj = oldAvatar.Avatar == _male ? eventListeners[1] : eventListeners[0];

        eventListeners[0].gameObject.SetActive(false);
        eventListeners[1].gameObject.SetActive(false);

        oldAvatarObj.gameObject.SetActive(false);
        newAvatarObj.gameObject.SetActive(true);

        oldAvatarObj.SetAvatar(newAvatar, newAvatarObj, entity);
    }
}
