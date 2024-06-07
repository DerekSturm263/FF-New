using Extensions.Types;
using UnityEngine;
using UnityEngine.Events;

public class SceneParameters : MonoBehaviour
{
    [SerializeField] private string _default;
    [SerializeField] private Dictionary<string, UnityEvent> _parameters;

    public void Invoke()
    {
        if (_parameters.TryGetValue(SceneParametersController.Instance.Parameter, out UnityEvent unityEvent))
            unityEvent.Invoke();
        else
            _parameters[_default].Invoke();
    }
}
