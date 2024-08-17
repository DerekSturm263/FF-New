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

    protected override RectTransform GetT(QuantumGame game, PlayerInfoCallbackContext ctx)
    {
        _numbers[ctx.Index.Global].SetActive(true);
        
        _numbers[ctx.Index.Global].GetComponentInChildren<TMPro.TMP_Text>().SetText(ctx.Index.Type == FighterType.Human ? $"P{ctx.Index.GlobalNoBots + 1}" : "Bot");
        _numbers[ctx.Index.Global].GetComponentInChildren<Image>().color = ctx.Index.Type == FighterType.Human ? _colors[ctx.Index.GlobalNoBots] : _botColor;
            
        return _numbers[ctx.Index.Global].GetComponent<RectTransform>();
    }

    protected override void CleanUp(RectTransform t)
    {
        t.gameObject.SetActive(false);
    }
}
