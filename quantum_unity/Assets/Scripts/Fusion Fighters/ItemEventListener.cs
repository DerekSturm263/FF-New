using Quantum;
using UnityEngine;

public class ItemEventListener : MonoBehaviour
{
    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnItemUse>(listener: this, handler: e =>
        {

        });
    }
}
