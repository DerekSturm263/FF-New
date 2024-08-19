using GameResources.Camera;
using Quantum;
using Quantum.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MatchEventListener : MonoBehaviour
{
    private EntityViewUpdater _entityView;

    [SerializeField] private GameObject _transition;
    [SerializeField] private float _delayTime;

    [SerializeField] private UnityEvent<QuantumGame, EntityViewUpdater, List<Team>> _onMatchStart;
    [SerializeField] private UnityEvent<QuantumGame, EntityViewUpdater, List<Team>, bool> _onMatchEnd;
    [SerializeField] private UnityEvent _onMatchEndDelayed;
    [SerializeField] private UnityEvent _onMatchEndDelayed2;

    [SerializeField] private UnityEvent _onMatchSetup;

    [SerializeField] private Image[] _runnerUpImages;
    [SerializeField] private Material[] _playerIconMats;

    private void Awake()
    {
        _entityView = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnMatchStart>(listener: this, handler: e =>
        {
            List<Team> teams = new();

            if (!e.Setup.Teams.Item1.Equals(default(Team)))
                teams.Add(e.Setup.Teams.Item1);
            if (!e.Setup.Teams.Item2.Equals(default(Team)))
                teams.Add(e.Setup.Teams.Item2);
            if (!e.Setup.Teams.Item3.Equals(default(Team)))
                teams.Add(e.Setup.Teams.Item3);
            if (!e.Setup.Teams.Item4.Equals(default(Team)))
                teams.Add(e.Setup.Teams.Item4);

            _onMatchStart.Invoke(e.Game, _entityView, teams);
        });

        QuantumEvent.Subscribe<EventOnMatchEnd>(listener: this, handler: e =>
        {
            List<Team> teams = new();

            if (!e.Results.Teams[0].Equals(default(Team)))
                teams.Add(e.Results.Teams[0]);
            if (!e.Results.Teams[1].Equals(default(Team)))
                teams.Add(e.Results.Teams[1]);
            if (!e.Results.Teams[2].Equals(default(Team)))
                teams.Add(e.Results.Teams[2]);
            if (!e.Results.Teams[3].Equals(default(Team)))
                teams.Add(e.Results.Teams[3]);

            _onMatchEnd.Invoke(e.Game, _entityView, teams, e.Results.WasForfeited);
        });

        QuantumEvent.Subscribe<EventOnMatchSetup>(listener: this, handler: e => _onMatchSetup.Invoke());
    }

    private (EntityViewUpdater entityViewUpdater, List<Team> teams, bool wasForfeited) matchResults;

    public void ZoomInOnWinners(QuantumGame game, EntityViewUpdater entityViewUpdater, List<Team> teams, bool wasForfeited)
    {
        matchResults = (entityViewUpdater, teams, wasForfeited);
    }

    public unsafe void LoadWinner(QuantumGame game, EntityViewUpdater entityViewUpdater, List<Team> teams, bool wasForfeited)
    {
        Invoke(nameof(LoadWinnerDelayed), _delayTime);
        Invoke(nameof(InvokeEventsDelayed), _delayTime + 1);
        Invoke(nameof(InvokeEventsDelayed2), _delayTime + 3);

        for (int i = 1; i < teams.Count; ++i)
        {
            var team = QuantumRunner.Default.Game.Frames.Verified.ResolveList(teams[i].Players);

            if (game.Frames.Verified.Unsafe.TryGetPointer(team[0], out PlayerStats* playerStats))
            {
                _runnerUpImages[i - 1].transform.parent.gameObject.SetActive(true);
                _runnerUpImages[i - 1].material = _playerIconMats[playerStats->Index.Global];
            }
        }

        for (int i = teams.Count; i < 4; ++i)
        {
            _runnerUpImages[i - 1].transform.parent.gameObject.SetActive(false);
        }
    }

    private void LoadWinnerDelayed()
    {
        _transition.SetActive(true);
    }

    private void InvokeEventsDelayed()
    {
        _onMatchEndDelayed.Invoke();

        QList<EntityRef> firstPlaceTeam = QuantumRunner.Default.Game.Frames.Verified.ResolveList(matchResults.teams[0].Players);
        CameraController.Instance.FocusTarget(QuantumRunner.Default.Game.Frames.Verified.Get<PlayerStats>(firstPlaceTeam[0]).Index.Global);
    }

    private void InvokeEventsDelayed2()
    {
        _onMatchEndDelayed2.Invoke();
    }

    public void ResetMatch()
    {
        CommandResetMatch command = new();
        QuantumRunner.Default.Game.SendCommand(command);
    }

    public void SetupMatch()
    {
        CommandSetupMatch command = new();
        QuantumRunner.Default.Game.SendCommand(command);
    }

    public void Quit()
    {
        CommandResetMatch command = new();
        QuantumRunner.Default.Game.SendCommand(command);
    }
}
