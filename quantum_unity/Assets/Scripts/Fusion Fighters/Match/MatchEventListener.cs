using GameResources.Camera;
using Quantum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MatchEventListener : MonoBehaviour
{
    private EntityViewUpdater _entityView;

    [SerializeField] private GameObject _transition;
    [SerializeField] private float _delayTime;
    [SerializeField] private CameraSettingsAsset _camSettings;

    [SerializeField] private UnityEvent<QuantumGame, EntityViewUpdater, List<Team>> _onMatchStart;
    [SerializeField] private UnityEvent<QuantumGame, EntityViewUpdater, List<Team>, bool> _onMatchEnd;

    private void Awake()
    {
        _entityView = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnMatchStart>(listener: this, handler: e =>
        {
            List<Team> teams = new();

            if (!e.Team1.Equals(default))
                teams.Add(e.Team1);
            if (!e.Team2.Equals(default))
                teams.Add(e.Team2);
            if (!e.Team3.Equals(default))
                teams.Add(e.Team3);
            if (!e.Team4.Equals(default))
                teams.Add(e.Team4);

            _onMatchStart.Invoke(e.Game, _entityView, teams);
        });

        QuantumEvent.Subscribe<EventOnMatchEnd>(listener: this, handler: e =>
        {
            List<Team> teams = new();

            if (!e.FirstPlace.Equals(default))
                teams.Add(e.FirstPlace);
            if (!e.SecondPlace.Equals(default))
                teams.Add(e.SecondPlace);
            if (!e.ThirdPlace.Equals(default))
                teams.Add(e.ThirdPlace);
            if (!e.LastPlace.Equals(default))
                teams.Add(e.LastPlace);

            _onMatchEnd.Invoke(e.Game, _entityView, teams, e.WasForfeited);
        });
    }

    private (QuantumGame game, EntityViewUpdater entityViewUpdater, List<Team> teams, bool wasForfeited) matchResults;

    public void ZoomInOnWinners(QuantumGame game, EntityViewUpdater entityViewUpdater, List<Team> teams, bool wasForfeited)
    {
        matchResults = (game, entityViewUpdater, teams, wasForfeited);
    }

    public void LoadWinner(QuantumGame game, EntityViewUpdater entityViewUpdater, List<Team> teams, bool wasForfeited)
    {
        Invoke(nameof(LoadWinnerDelayed), _delayTime);
        Invoke(nameof(SetCameraDelayed), _delayTime + 1);
    }

    private void LoadWinnerDelayed()
    {
        _transition.SetActive(true);
    }

    private void SetCameraDelayed()
    {
        var firstPlaceTeam = matchResults.game.Frames.Verified.ResolveList(matchResults.teams[0].Players);
        CameraController.Instance.FocusTarget(firstPlaceTeam[0].Player._index - 1);

        CameraController.Instance.SetCameraSettings(_camSettings);
    }
}
