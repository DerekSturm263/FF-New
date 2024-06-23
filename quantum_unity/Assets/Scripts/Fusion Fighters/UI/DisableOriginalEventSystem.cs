using UnityEngine;
using UnityEngine.EventSystems;

public class DisableOriginalEventSystem : MonoBehaviour
{
    private EventSystem _old;

    private void OnEnable()
    {
        _old = EventSystem.current;
        _old.enabled = false;
    }

    private void OnDisable()
    {
        _old.enabled = true;
        EventSystem.current = _old;
    }
}
