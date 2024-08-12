using Quantum;
using UnityEngine;
using UnityEngine.Rendering;

public class ShowLightsOnCamera : MonoBehaviour
{
    [SerializeField] private Light[] _includeLights;
    private Light[] _excludeLights = new Light[0];

    private Camera _camera;

    private EntityViewUpdater _entityViewUpdater;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        unsafe
        {
            if (QuantumRunner.Default is not null && QuantumRunner.Default.Game.Frames.Verified.Global->CurrentStage.IsValid)
            {
                _excludeLights = _entityViewUpdater.GetView(QuantumRunner.Default.Game.Frames.Verified.Global->CurrentStage).GetComponentsInChildren<Light>(true);
            }
        }

        QuantumEvent.Subscribe<EventOnStageSelect>(listener: this, handler: LoadLights);

        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    private void LoadLights(EventOnStageSelect e)
    {
        _excludeLights = _entityViewUpdater.GetView(e.Stage).GetComponentsInChildren<Light>(true);
    }

    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 != _camera)
            return;

        for (int i = 0; i < _excludeLights.Length; ++i)
        {
            _excludeLights[i].enabled = false;
        }

        for (int i = 0; i < _includeLights.Length; ++i)
        {
            _includeLights[i].enabled = true;
        }
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 != _camera)
            return;

        for (int i = 0; i < _excludeLights.Length; ++i)
        {
            _excludeLights[i].enabled = true;
        }

        for (int i = 0; i < _includeLights.Length; ++i)
        {
            _includeLights[i].enabled = false;
        }
    }
}
