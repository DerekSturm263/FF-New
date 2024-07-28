using GameResources.Camera;
using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumController : PlayerTracker<RectTransform>
{
    [SerializeField] private GameObject[] _numbers;
    [SerializeField] private Color[] _colors;
    [SerializeField] private Color _botColor;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private Canvas _canvas;

    protected override void Action(GameObject player, RectTransform t)
    {
        if (CameraController.Instance)
            t.anchoredPosition = (CameraController.Instance.Cam.WorldToScreenPoint(player.transform.position + (Vector3)_offset) - (new Vector3(Screen.width, Screen.height) / 2)) / _canvas.scaleFactor;
    }

    protected override RectTransform GetT(QuantumGame game, EntityRef player, QString32 name, FighterIndex index)
    {
        _numbers[index.Global].SetActive(true);
        
        _numbers[index.Global].GetComponentInChildren<TMPro.TMP_Text>().SetText(index.Type == FighterType.Human ? $"P{index.GlobalNoBots + 1}" : "Bot");
        _numbers[index.Global].GetComponentInChildren<Image>().color = index.Type == FighterType.Human ? _colors[index.GlobalNoBots] : _botColor;
            
        return _numbers[index.Global].GetComponent<RectTransform>();
    }

    protected override void CleanUp(RectTransform t)
    {
        t.gameObject.SetActive(false);
    }
}
