using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private TMPro.TMP_Text _label;

    [SerializeField] private Animator _anim;
    [SerializeField] private CanvasGroup _canvasGroup;

    private RectTransform _rect;

    private LocalPlayerInfo _playerInfo;
    public LocalPlayerInfo PlayerInfo => _playerInfo;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UpdateColors();
    }

    public void Initialize(LocalPlayerInfo playerInfo)
    {
        _playerInfo = playerInfo;
        UpdateColors();
    }

    public void UpdateColors()
    {
        if (!QuantumRunner.Default || !QuantumRunner.Default.IsRunning)
            return;

        if (QuantumRunner.Default.Game.Frames.Verified.TryGet(FighterIndex.GetPlayerFromIndex(QuantumRunner.Default.Game.Frames.Verified, _playerInfo.Index), out Quantum.CharacterController characterController) && characterController.IsReady)
        {
            _canvasGroup.alpha = 0.5f;
            _anim.enabled = false;
        }
        else
        {
            _canvasGroup.alpha = 1;
            _anim.enabled = true;
        }

        _arrow.color = _playerInfo.Index.GetLightColor(QuantumRunner.Default.Game.Frames.Verified).ToColor();
        _label.SetText($"P{_playerInfo.Index.GlobalNoBots + 1}");
    }

    public void ChildToSelected(GameObject selected)
    {
        _rect ??= GetComponent<RectTransform>();

        transform.SetParent(selected.transform);

        _rect.anchoredPosition = new(-30, 8);
        _rect.anchorMin = new(1, 1);
        _rect.anchorMax = new(1, 1);
    }
}
