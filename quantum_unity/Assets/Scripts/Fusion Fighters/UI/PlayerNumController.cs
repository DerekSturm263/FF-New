using GameResources.Camera;
using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumController : PlayerTracker<RectTransform>
{
    [SerializeField] private GameObject[] _numbers;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private Canvas _canvas;

    private static PlayerNumController _instance;

    protected override void Awake()
    {
        _instance = this;

        base.Awake();
        
        QuantumEvent.Subscribe<EventOnPlayerChangeTeam>(listener: this, handler: e => _numbers[e.Index.Global].GetComponentInChildren<Image>().color = e.Index.GetLightColor(QuantumRunner.Default.Game.Frames.Verified).ToColor());
    }

    protected override void Action(GameObject player, RectTransform t)
    {
        if (CameraController.Instance)
            t.anchoredPosition = (CameraController.Instance.Cam.WorldToScreenPoint(player.transform.position + (Vector3)_offset) - (new Vector3(Screen.width, Screen.height) / 2)) / _canvas.scaleFactor;
    }

    protected override RectTransform GetT(QuantumGame game, PlayerInfoCallbackContext ctx)
    {
        _numbers[ctx.Index.Global].SetActive(true);
        
        _numbers[ctx.Index.Global].GetComponentInChildren<TMPro.TMP_Text>().SetText(ctx.Index.Type == FighterType.Human ? $"P{ctx.Index.GlobalNoBots + 1}" : "Bot");
        _numbers[ctx.Index.Global].GetComponentInChildren<Image>().color = ctx.Index.GetLightColor(QuantumRunner.Default.Game.Frames.Verified).ToColor();
            
        return _numbers[ctx.Index.Global].GetComponent<RectTransform>();
    }

    protected override void CleanUp(RectTransform t)
    {
        t.gameObject.SetActive(false);
    }

    public void SetEnabled(bool isEnabled)
    {
        if (!_instance)
            return;

        _instance.gameObject.SetActive(isEnabled);
    }
}
