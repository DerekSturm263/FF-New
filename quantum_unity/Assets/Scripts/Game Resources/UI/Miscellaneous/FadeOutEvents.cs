using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator), typeof(CanvasGroup))]
public class FadeOutEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent _onOpen;
    public void InvokeOnOpen() => _onOpen.Invoke();

    [SerializeField] private UnityEvent _onClose;
    public void InvokeOnClose() => _onClose.Invoke();

    private Button[] _buttons;

    private void OnEnable()
    {
        _buttons = GetComponentsInChildren<Button>();
        InvokeOnOpen();
    }

    public void DisableButtons()
    {
        foreach (Button button in _buttons)
        {
            button.interactable = false;
        }
    }
}
