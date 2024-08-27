using Extensions.Miscellaneous;
using GameResources.Camera;
using Quantum;
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
    [SerializeField] private TMPro.TMP_Text _text;
    [SerializeField] private Image[] _runnerUpFrames;
    [SerializeField] private Image[] _runnerUpImages;
    [SerializeField] private TMPro.TMP_Text[] _runnerUpNames;
    [SerializeField] private TMPro.TMP_Text[] _runnerUpPlaces;
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
        _line.color = winningTeam.ElementAt(0).Index.GetDarkColor(QuantumRunner.Default.Game.Frames.Verified).ToColor();
        _line.color = new(_line.color.r, _line.color.g, _line.color.b, 0.75f);

        string winningPlayers = Helper.PrintNames(winningTeam, item => item.Name, "No one");

        if (winningTeam.Count() == 1)
            _text.SetText($"{winningPlayers} wins!");
        else
            _text.SetText($"{winningPlayers} win!");

        IEnumerable<PlayerNameIndex> runnerUps = teams.Where(item => !item.Equals(teams.ElementAt(0))).Select(item => item.Get(QuantumRunner.Default.Game.Frames.Verified)).SelectMany(item => item.ToList());

        for (int i = 0; i < runnerUps.Count(); ++i)
        {
            _runnerUpFrames[i].transform.parent.gameObject.SetActive(true);

            _runnerUpFrames[i].color = runnerUps.ElementAt(i).Index.GetLightColor(QuantumRunner.Default.Game.Frames.Verified).ToColor();
            _runnerUpImages[i].material = _playerIconMats[runnerUps.ElementAt(i).Index.Global];
            _runnerUpNames[i].SetText(runnerUps.ElementAt(i).Name);
            _runnerUpPlaces[i].SetText(i == 0 ? "2<sup>nd</sup>" : i == 1 ? "3<sup>rd</sup>" : "4<sup>th</sup>");
        }

        for (int i = runnerUps.Count(); i < 3; ++i)
        {
            _runnerUpFrames[i].transform.parent.gameObject.SetActive(false);
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
        CameraController.Instance.FocusTarget(firstPlaceTeam.ElementAt(0).Index.Global);
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
