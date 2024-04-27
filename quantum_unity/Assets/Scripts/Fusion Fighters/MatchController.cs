using Quantum;
using Quantum.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MatchController : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerReady;
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerCancel;
    [SerializeField] private UnityEvent<QuantumGame> _onAllPlayersReady;

    [SerializeField] private UnityEvent<QuantumGame, QList<Team>> _onMatchStart;
    [SerializeField] private UnityEvent<QuantumGame, QList<Team>> _onMatchWin;
    [SerializeField] private UnityEvent<QuantumGame, QList<Team>> _onMatchLose;
    [SerializeField] private UnityEvent<QuantumGame, QList<Team>> _onMatchForfeit;

    private void Awake()
    {
        QuantumEvent.Subscribe<EventOnPlayerReady>(listener: this, handler: e => _onPlayerReady.Invoke(e.Game, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerCancel>(listener: this, handler: e => _onPlayerCancel.Invoke(e.Game, e.Player));
        QuantumEvent.Subscribe<EventOnAllPlayersReady>(listener: this, handler: e => _onAllPlayersReady.Invoke(e.Game));

        /*QuantumEvent.Subscribe<EventOnMatchStart>(listener: this, handler: e =>
        {
            QList<Team> teams = e.Game.Frames.Verified.ResolveList<Team>(e.Teams);
            _onMatchStart.Invoke(teams);
        });

        QuantumEvent.Subscribe<EventOnMatchWin>(listener: this, handler: e =>
        {
            QList<Team> teams = e.Game.Frames.Verified.ResolveList<Team>(e.Winners);
            _onMatchWin.Invoke(teams);
        });

        QuantumEvent.Subscribe<EventOnMatchLose>(listener: this, handler: e =>
        {
            QList<Team> teams = e.Game.Frames.Verified.ResolveList<Team>(e.Losers);
            _onMatchLose.Invoke(teams);
        });

        QuantumEvent.Subscribe<EventOnMatchForfeit>(listener: this, handler: e =>
        {
            QList<Team> teams = e.Game.Frames.Verified.ResolveList<Team>(e.Teams);
            _onMatchForfeit.Invoke(teams);
        });*/
    }
}
