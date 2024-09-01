using Extensions.Components.Miscellaneous;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExcludeGameObjectsFromCamera : Controller<ExcludeGameObjectsFromCamera>
{
    [SerializeField] private List<GameObject> _exclude;
    public void SetExclude(List<GameObject> exclude) => _exclude = exclude;
    public void ClearExclude() => _instance._exclude.Clear();

    private Camera _camera;

    private void Awake()
    {
        Initialize();

        _camera = GetComponent<Camera>();

        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 != _camera)
            return;

        for (int i = 0; i < _exclude.Count; ++i)
        {
            _exclude[i].SetActive(false);
        }
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 != _camera)
            return;

        for (int i = 0; i < _exclude.Count; ++i)
        {
            _exclude[i].SetActive(true);
        }
    }
}
