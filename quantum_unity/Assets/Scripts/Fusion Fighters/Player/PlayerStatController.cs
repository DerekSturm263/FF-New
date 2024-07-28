using Quantum;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField] private HUDPlayerLink[] _huds;
    public HUDPlayerLink[] HUDS => _huds;

    public static PlayerStatController Instance;

    public void Initialize(QuantumGame game, EntityRef player, QString32 name, FighterIndex index)
    {
        _huds[index.Global].gameObject.SetActive(true);

        _huds[index.Global].SetPlayerName(name);
        _huds[index.Global].SetPlayerNumber(index);
    }

    public void Destroy(QuantumGame game, EntityRef player, QString32 name, FighterIndex index)
    {
        _huds[index.Global].gameObject.SetActive(false);
    }

    private void Awake()
    {
        Instance = this;

        QuantumEvent.Subscribe<EventOnPlayerSetName>(listener: this, handler: (e) => _huds[e.Index.Global].SetPlayerName(e.Name));
        QuantumEvent.Subscribe<EventOnPlayerReady>(listener: this, handler: (e) => _huds[e.Index.Global].UpdateReadiness(true));
        QuantumEvent.Subscribe<EventOnPlayerCancel>(listener: this, handler: (e) => _huds[e.Index.Global].UpdateReadiness(false));
        QuantumEvent.Subscribe<EventOnPlayerUpdateReady>(listener: this, handler: (e) => _huds[e.Index.Global].UpdateReadinessValue(e.Readiness.AsFloat));
        QuantumEvent.Subscribe<EventOnPlayerModifyHealth>(listener: this, handler: (e) => _huds[e.Index.Global].UpdateHealth(e.NewHealth, e.MaxHealth));
        QuantumEvent.Subscribe<EventOnPlayerModifyEnergy>(listener: this, handler: (e) => _huds[e.Index.Global].UpdateEnergy(e.NewEnergy, e.MaxEnergy));
        QuantumEvent.Subscribe<EventOnPlayerModifyStocks>(listener: this, handler: (e) => _huds[e.Index.Global].UpdateStocks(e.NewStocks, e.MaxStocks));
        QuantumEvent.Subscribe<EventOnHideShowReadiness>(listener: this, handler: (e) => _huds[e.Index.Global].ShowReadiness(e.ShowReadiness && e.Index.Type == FighterType.Human));
    }

    public void ShowAllReadies(bool doShow)
    {
        foreach (HUDPlayerLink playerHUD in _huds)
        {
            playerHUD.ShowReadiness(doShow);
        }
    }
}
