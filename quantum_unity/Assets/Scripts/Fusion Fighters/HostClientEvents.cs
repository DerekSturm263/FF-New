using Quantum.Demo;
using UnityEngine;
using UnityEngine.Events;

public class HostClientEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent _onHost;
    [SerializeField] private UnityEvent _onClient;

    [SerializeField] private UnityEvent _load;

    private void Awake()
    {
        if (UIMain.Client is not null)
        {
            _load.Invoke();

            if (UIMain.Client.LocalPlayer.IsMasterClient)
                _onHost.Invoke();
            else
                _onClient.Invoke();
        }
    }

    public void Invoke()
    {
        _onHost.Invoke();
    }
}
