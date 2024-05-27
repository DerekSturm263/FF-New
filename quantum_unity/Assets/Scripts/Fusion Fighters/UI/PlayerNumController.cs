using GameResources.Camera;
using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumController : PlayerTracker<RectTransform>
{
    [SerializeField] private GameObject[] _numbers;
    [SerializeField] private Color[] _colors;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private Canvas _canvas;

    protected override void Action(EntityView player, RectTransform t)
    {
        if (CameraController.Instance)
            t.anchoredPosition = (CameraController.Instance.Cam.WorldToScreenPoint(player.transform.position + (Vector3)_offset) - (new Vector3(Screen.width, Screen.height) / 2)) / _canvas.scaleFactor;
    }

    protected override RectTransform GetT(QuantumGame game, PlayerLink player)
    {
        _numbers[player.Player._index - 1].SetActive(true);
        _numbers[player.Player._index - 1].GetComponentInChildren<TMPro.TMP_Text>().SetText($"P{player.Player._index}");
        _numbers[player.Player._index - 1].GetComponentInChildren<Image>().color = _colors[player.Player._index - 1];

        return _numbers[player.Player._index - 1].GetComponent<RectTransform>();
    }

    protected override void CleanUp(RectTransform t)
    {
        t.gameObject.SetActive(false);
    }
}
