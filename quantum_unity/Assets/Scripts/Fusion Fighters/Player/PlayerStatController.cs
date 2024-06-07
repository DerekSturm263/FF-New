using Quantum;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField] private HUDPlayerLink[] _huds;
    public HUDPlayerLink[] HUDS => _huds;

    public static PlayerStatController Instance;

    public void Initialize(QuantumGame game, EntityRef player, int index)
    {
        _huds[index].gameObject.SetActive(true);
        _huds[index].SetPlayerNumber(index + 1);
    }

    public void Destroy(QuantumGame game, EntityRef player, int index)
    {
        _huds[index].gameObject.SetActive(false);
    }

    private void Awake()
    {
        Instance = this;

        QuantumEvent.Subscribe<EventOnPlayerReady>(listener: this, handler: (e) => _huds[e.Index].UpdateReadiness(true));
        QuantumEvent.Subscribe<EventOnPlayerCancel>(listener: this, handler: (e) => _huds[e.Index].UpdateReadiness(false));
        QuantumEvent.Subscribe<EventOnPlayerUpdateReady>(listener: this, handler: (e) => _huds[e.Index].UpdateReadinessValue(e.Readiness.AsFloat));
        QuantumEvent.Subscribe<EventOnPlayerModifyHealth>(listener: this, handler: (e) => _huds[e.Index].UpdateHealth(e.NewHealth, e.MaxHealth));
        QuantumEvent.Subscribe<EventOnPlayerModifyEnergy>(listener: this, handler: (e) => _huds[e.Index].UpdateEnergy(e.NewEnergy, e.MaxEnergy));
        QuantumEvent.Subscribe<EventOnPlayerModifyStocks>(listener: this, handler: (e) => _huds[e.Index].UpdateStocks(e.NewStocks, e.MaxStocks));
    }

    public void ShowAllReadies(bool show)
    {
        foreach (HUDPlayerLink playerHUD in _huds)
        {
            playerHUD.ShowReadiness(show);
        }
    }
}
