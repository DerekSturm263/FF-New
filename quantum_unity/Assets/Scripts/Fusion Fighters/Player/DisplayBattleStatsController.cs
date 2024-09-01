using Quantum;
using UnityEngine;

public class DisplayBattleStatsController : MonoBehaviour
{
    private DisplayBattleStats[] _displays;

    private void Awake()
    {
        _displays = GetComponentsInChildren<DisplayBattleStats>(true);
    }

    private void OnEnable()
    {
        foreach (var display in _displays)
        {
            display.gameObject.SetActive(!FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, item => item.Global == display.GlobalIndex).Equals(FighterIndex.Invalid));
        }
    }
}
