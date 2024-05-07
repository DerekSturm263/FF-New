using Quantum;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField] private HUDPlayerLink[] _huds;

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
        QuantumEvent.Subscribe<EventOnPlayerModifyHealth>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateHealth(e.NewHealth, e.MaxHealth));
        QuantumEvent.Subscribe<EventOnPlayerModifyEnergy>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateEnergy(e.NewEnergy, e.MaxEnergy));
        QuantumEvent.Subscribe<EventOnPlayerModifyStocks>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateStocks(e.NewStocks, e.MaxStocks));
    }
}
