using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class RulesetEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<Ruleset> _onSwitch;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnRulesetSelect>(listener: this, handler: e =>
        {
            _onSwitch.Invoke(e.New);
        });
    }
}
