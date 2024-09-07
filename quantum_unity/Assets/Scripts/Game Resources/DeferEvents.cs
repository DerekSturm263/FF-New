using Quantum;
using UnityEngine;

public class DeferEvents : MonoBehaviour
{
    public static System.Action<QuantumGame> OnStartGlobal;

    private void Start()
    {
        if (OnStartGlobal is not null && QuantumRunner.Default.IsRunning)
        {
            OnStartGlobal.Invoke(QuantumRunner.Default.Game);
            OnStartGlobal = default;
        }
    }

    public static void Defer(System.Action<QuantumGame> eventAction)
    {
        OnStartGlobal = eventAction;
    }
}
