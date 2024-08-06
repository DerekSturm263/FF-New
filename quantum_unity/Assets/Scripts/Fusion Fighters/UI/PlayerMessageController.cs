using GameResources.Camera;
using Quantum;
using UnityEngine;

public class PlayerMessageController : PlayerTracker<RectTransform>
{
    [SerializeField] private GameObject[] _messageBubbles;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private Canvas _canvas;

    protected override void Awake()
    {
        base.Awake();

        QuantumEvent.Subscribe<EventOnSendMessage>(listener: this, handler: e =>
        {
            _messageBubbles[e.Index.Global].SetActive(true);
            _messageBubbles[e.Index.Global].GetComponentInChildren<TMPro.TMP_Text>().SetText(e.Message);
        });
    }

    protected override void Action(GameObject player, RectTransform t)
    {
        if (CameraController.Instance)
            t.anchoredPosition = (CameraController.Instance.Cam.WorldToScreenPoint(player.transform.position + (Vector3)_offset) - (new Vector3(Screen.width, Screen.height) / 2)) / _canvas.scaleFactor;
    }

    protected override RectTransform GetT(QuantumGame game, EntityRef player, QString32 name, FighterIndex index)
    {
        return _messageBubbles[index.Global].GetComponent<RectTransform>();
    }

    protected override void CleanUp(RectTransform t)
    {
        t.gameObject.SetActive(false);
    }
}
