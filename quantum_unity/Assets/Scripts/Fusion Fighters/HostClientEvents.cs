using UnityEngine;
using UnityEngine.Events;

public class HostClientEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent _onHost;
    [SerializeField] private UnityEvent _onClient;

    public void Invoke()
    {
        //if (QuantumRunner.Default.NetworkClient.)
    }
}
