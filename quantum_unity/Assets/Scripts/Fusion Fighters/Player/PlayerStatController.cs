using Quantum;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField] private HUDPlayerLink[] _huds;
    public HUDPlayerLink[] HUDS => _huds;

    public static PlayerStatController Instance;

    public void Initialize(QuantumGame game, PlayerLink player)
    {
        _huds[player.Player._index - 1].gameObject.SetActive(true);
        _huds[player.Player._index - 1].SetPlayerNumber(player.Player._index);
    }

    public void Destroy(QuantumGame game, PlayerLink player)
    {
        _huds[player.Player._index - 1].gameObject.SetActive(false);
    }

    private void Awake()
    {
        Instance = this;

        QuantumEvent.Subscribe<EventOnPlayerReady>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateReadiness(true));
        QuantumEvent.Subscribe<EventOnPlayerCancel>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateReadiness(false));
        QuantumEvent.Subscribe<EventOnPlayerModifyHealth>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateHealth(e.NewHealth, e.MaxHealth));
        QuantumEvent.Subscribe<EventOnPlayerModifyEnergy>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateEnergy(e.NewEnergy, e.MaxEnergy));
        QuantumEvent.Subscribe<EventOnPlayerModifyStocks>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateStocks(e.NewStocks, e.MaxStocks));
    }

    public void ShowAllReadies(bool show)
    {
        foreach (HUDPlayerLink playerHUD in _huds)
        {
            playerHUD.ShowReadiness(show);
        }
    }
}
