using GameResources;
using UnityEngine;

public class ConditionalEnable : MonoBehaviour
{
    [SerializeField] private Behaviour _component;

    private void OnEnable()
    {
        _component.enabled = VFXController.Instance.IsEnabled;
    }
}
