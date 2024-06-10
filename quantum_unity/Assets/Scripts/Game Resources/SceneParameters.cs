using Extensions.Types;
using UnityEngine;
using UnityEngine.Events;

public class SceneParameters : MonoBehaviour
{
    [SerializeField] private string _default;
    [SerializeField] private Dictionary<string, UnityEvent> _parameters;

    public void Invoke()
    {
        if (SceneParametersController.Instance.Parameter is not null && _parameters.TryGetValue(SceneParametersController.Instance.Parameter, out UnityEvent unityEvent))
            unityEvent.Invoke();
        else
            if (_parameters.TryGetValue(_default, out UnityEvent value))
                value.Invoke();
    }
}
