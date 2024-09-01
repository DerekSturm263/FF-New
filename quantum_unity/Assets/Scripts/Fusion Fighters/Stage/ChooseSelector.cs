using Quantum;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class ChooseSelector : MonoBehaviour
{
    [SerializeField] private GameObject _stages;
    [SerializeField] private bool _isStageSelector;

    private BindParent _binding;
    public BindParent Binding => _binding;

    public bool IsActive => _stages.activeInHierarchy;

    private void Awake()
    {
        _binding = GetComponent<BindParent>();
    }

    private void OnEnable()
    {
        if (_isStageSelector)
        {
            StagePicker stagePicker = QuantumRunner.Default.Game.Frames.Verified.FindAsset<StagePicker>(RulesetController.Instance.CurrentRuleset.value.Stage.StagePicker.Id);
            var (unsorted, sorted) = MatchSystem.GetTeams(QuantumRunner.Default.Game.Frames.Verified);

            bool canSelect = (sorted.Count() == 0 && stagePicker.GetInitialPickers(QuantumRunner.Default.Game.Frames.Verified, unsorted).Any(item => item.Get(QuantumRunner.Default.Game.Frames.Verified).Any(item => item.Index.Equals(_binding.Player.Index)))) ||
                (sorted.Count() != 0 && stagePicker.GetAllowedPickers(QuantumRunner.Default.Game.Frames.Verified, sorted).Any(item => item.Get(QuantumRunner.Default.Game.Frames.Verified).Any(item => item.Index.Equals(_binding.Player.Index))));

            SetActive(canSelect);
        }
    }

    public void SetActive(bool isActive)
    {
        _stages.SetActive(isActive);
        GetComponentInChildren<Selector>(true).gameObject.SetActive(isActive);

        if (!isActive)
            _binding.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(null);
    }
}
