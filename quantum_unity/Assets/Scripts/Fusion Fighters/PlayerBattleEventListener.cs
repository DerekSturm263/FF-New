using Quantum;
using UnityEngine;

public class PlayerBattleEventListener : MonoBehaviour
{
    [SerializeField] private HUDPlayerLink[] _huds;

    public void Initialize(QuantumGame game, PlayerLink player)
    {
        _huds[player.Player._index - 1].gameObject.SetActive(true);
    }

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnPlayerModifyHealth>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateHealth(e.NewHealth, e.MaxHealth));
        QuantumEvent.Subscribe<EventOnPlayerModifyEnergy>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateEnergy(e.NewEnergy, e.MaxEnergy));
        QuantumEvent.Subscribe<EventOnPlayerModifyStocks>(listener: this, handler: (e) => _huds[e.Player.Player._index - 1].UpdateStocks(e.NewStocks, e.MaxStocks));
    }
}
