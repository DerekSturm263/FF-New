using UnityEngine;
using Extensions.Components.Miscellaneous;

public abstract class SpawnableController<T> : Controller<SpawnableController<T>>
{
    [SerializeField] protected GameObject _template;

    protected GameObject _templateInstance;
    protected T _t;

    protected abstract bool TakeAwayFocus();

    public void Spawn(T t)
    {
        if (TakeAwayFocus())
            EventSystemController.Instance.Enable();

        Transform parent = GameObject.FindWithTag("Popup Canvas").transform;
        _templateInstance = Instantiate(_template, parent);

        _t = t;
        SetUp(_t);
    }

    protected virtual void SetUp(T t) { }

    public void Despawn()
    {
        CleanUp(_t);

        _templateInstance.GetComponent<Animator>().SetTrigger("Exit");

        if (_templateInstance.TryGetComponent(out FadeOutEvents fadeOutEvents))
        {
            fadeOutEvents.InvokeOnClose();
            fadeOutEvents.DisableButtons();
        }

        if (TakeAwayFocus())
            EventSystemController.Instance.Disable();
    }

    protected virtual void CleanUp(T t) { }
}
