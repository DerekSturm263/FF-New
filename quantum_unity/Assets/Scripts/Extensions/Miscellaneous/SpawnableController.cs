using UnityEngine;
using Extensions.Components.Miscellaneous;

public abstract class SpawnableController<T> : Controller<SpawnableController<T>>
{
    [SerializeField] private GameObject _template;

    protected GameObject _templateInstance;
    private T _t;

    public void Spawn(T t)
    {
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

        EventSystemController.Instance.Disable();
    }

    protected virtual void CleanUp(T t) { }
}
