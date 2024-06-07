using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class StageEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<Stage> _onSwitch;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnStageSelect>(listener: this, handler: e =>
        {
            _onSwitch.Invoke(e.New);
        });
    }

    public void SetFogSettings(Stage stage)
    {
        RenderSettings.fogColor = stage.Theme.FogColor.ToColor();
        RenderSettings.fogDensity = stage.Theme.FogDensity.AsFloat;
    }
}
