using Quantum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShowLightsOnCamera : MonoBehaviour
{
    [SerializeField] private Light[] _includeLights;

    private Light[] _excludeLights = new Light[0];
    private List<GameObject> _excludeGameObjects = new();

    private Camera _camera;

    private EntityViewUpdater _entityViewUpdater;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        unsafe
        {
            if (QuantumRunner.Default)
            {
                if (QuantumRunner.Default.Game.Frames.Verified.Global->CurrentStage.IsValid)
                    _excludeLights = _entityViewUpdater.GetView(QuantumRunner.Default.Game.Frames.Verified.Global->CurrentStage).GetComponentsInChildren<Light>(true);

                var players = FighterIndex.GetEntityList(QuantumRunner.Default.Game.Frames.Verified, item => true);

                foreach (var playerEty in players)
                {
                    GameObject player = _entityViewUpdater.GetView(playerEty).gameObject;

                    if (player != transform.parent.gameObject)
                        _excludeGameObjects.Add(player);
                }
            }
        }

        QuantumEvent.Subscribe<EventOnStageSelect>(listener: this, handler: LoadLights);
        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: AddPlayer);
        QuantumEvent.Subscribe<EventOnPlayerDespawn>(listener: this, handler: RemovePlayer);

        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    private void LoadLights(EventOnStageSelect e)
    {
        _excludeLights = _entityViewUpdater.GetView(e.Stage).GetComponentsInChildren<Light>(true);
    }

    private void AddPlayer(EventOnPlayerSpawn e)
    {
        _excludeGameObjects.Add(_entityViewUpdater.GetView(e.Ctx.Entity).gameObject);
    }

    private void RemovePlayer(EventOnPlayerDespawn e)
    {
        //_excludeGameObjects.Remove(_entityViewUpdater.GetView(e.Ctx.Entity).gameObject);
    }

    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 != _camera)
            return;

        for (int i = 0; i < _excludeLights.Length; ++i)
        {
            if (_excludeLights[i])
                _excludeLights[i].enabled = false;
        }

        for (int i = 0; i < _excludeGameObjects.Count; ++i)
        {
            if (_excludeGameObjects[i])
                _excludeGameObjects[i].SetActive(false);
        }

        for (int i = 0; i < _includeLights.Length; ++i)
        {
            if (_includeLights[i])
                _includeLights[i].enabled = true;
        }
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 != _camera)
            return;

        for (int i = 0; i < _excludeLights.Length; ++i)
        {
            if (_excludeLights[i])
                _excludeLights[i].enabled = true;
        }

        for (int i = 0; i < _excludeGameObjects.Count; ++i)
        {
            if (_excludeGameObjects[i])
                _excludeGameObjects[i].SetActive(true);
        }

        for (int i = 0; i < _includeLights.Length; ++i)
        {
            if (_includeLights[i])
                _includeLights[i].enabled = false;
        }
    }
}
