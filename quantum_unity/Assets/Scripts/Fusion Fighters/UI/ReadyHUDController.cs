using GameResources.Camera;
using Quantum;
using UnityEngine;

public class ReadyHUDController : PlayerTracker<RectTransform>
{
    [SerializeField] private GameObject[] _readies;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private Canvas _canvas;

    protected override void Action(EntityView player, RectTransform t)
    {
        if (CameraController.Instance)
            t.anchoredPosition = (CameraController.Instance.Cam.WorldToScreenPoint(player.transform.position + (Vector3)_offset) - (new Vector3(Screen.width, Screen.height) / 2)) / _canvas.scaleFactor;
    }

    protected override RectTransform GetT(QuantumGame game, EntityRef player, int index)
    {
        _readies[index].SetActive(true);
        return _readies[index].GetComponent<RectTransform>();
    }

    protected override void CleanUp(RectTransform t)
    {
        t.gameObject.SetActive(false);
    }

    public void SetPlayerReady(QuantumGame game, EntityRef player, int index)
    {
        _readies[index].transform.GetChild(0).gameObject.SetActive(false);
        _readies[index].transform.GetChild(1).gameObject.SetActive(true);
    }

    public void SetPlayerUnready(QuantumGame game, EntityRef player, int index)
    {
        _readies[index].transform.GetChild(0).gameObject.SetActive(true);
        _readies[index].transform.GetChild(1).gameObject.SetActive(false);
    }
}
