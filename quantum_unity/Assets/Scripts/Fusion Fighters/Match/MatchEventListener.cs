using GameResources.Camera;
using Quantum;
using Quantum.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MatchEventListener : MonoBehaviour
{
    private EntityViewUpdater _entityViewUpdater;

    [SerializeField] private GameObject _transition;
    [SerializeField] private float _delayTime;

    [SerializeField] private UnityEvent<EventOnMatchStart> _onMatchStart;
    [SerializeField] private UnityEvent<EventOnMatchEnd> _onMatchEnd;
    [SerializeField] private UnityEvent _onMatchEndDelayed;
    [SerializeField] private UnityEvent _onMatchEndDelayed2;

    [SerializeField] private UnityEvent _onMatchSetup;

    [SerializeField] private Image _line;
    [SerializeField] private Image[] _runnerUpImages;
    [SerializeField] private Material[] _playerIconMats;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnMatchStart>(listener: this, handler: _onMatchStart.Invoke);
        QuantumEvent.Subscribe<EventOnMatchEnd>(listener: this, handler: _onMatchEnd.Invoke);

        QuantumEvent.Subscribe<EventOnMatchSetup>(listener: this, handler: e => _onMatchSetup.Invoke());
    }

    private MatchResults matchResults;

    public void ZoomInOnWinners(EventOnMatchEnd e)
    {
        matchResults = e.Results;
    }

    public unsafe void LoadWinner(EventOnMatchEnd e)
    {
        Invoke(nameof(LoadWinnerDelayed), _delayTime);
        Invoke(nameof(InvokeEventsDelayed), _delayTime + 1);
        Invoke(nameof(InvokeEventsDelayed2), _delayTime + 3);

        var teams = e.Results.SortedTeams.Get(QuantumRunner.Default.Game.Frames.Verified);

        var winningTeam = teams.ElementAt(0).Get(QuantumRunner.Default.Game.Frames.Verified);
        _line.color = winningTeam.ElementAt(0).GetDarkColor(QuantumRunner.Default.Game.Frames.Verified).ToColor();

        for (int i = 1; i < teams.Count(); ++i)
        {
            var team = teams.ElementAt(i).Get(QuantumRunner.Default.Game.Frames.Verified);

            _runnerUpImages[i - 1].transform.parent.gameObject.SetActive(true);
            _runnerUpImages[i - 1].material = _playerIconMats[team.ElementAt(0).Global];
        }

        for (int i = teams.Count(); i < 4; ++i)
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

        var teams = matchResults.SortedTeams.Get(QuantumRunner.Default.Game.Frames.Verified);

        var firstPlaceTeam = teams.ElementAt(0).Get(QuantumRunner.Default.Game.Frames.Verified);
        CameraController.Instance.FocusTarget(firstPlaceTeam.ElementAt(0).Global);
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
