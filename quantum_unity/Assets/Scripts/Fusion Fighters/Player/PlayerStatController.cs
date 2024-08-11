using Extensions.Components.Miscellaneous;
using Quantum;
using System.Collections.Generic;
using System.Linq;

public class PlayerStatController : Controller<PlayerStatController>
{
    private List<HUDPlayerLink[]> _huds;
    public List<HUDPlayerLink[]> HUDS => _huds;

    public void Initialize(QuantumGame game, PlayerInfoCallbackContext ctx)
    {
        _huds.ForEach(item => item[ctx.Index.Global].gameObject.SetActive(true));

        _huds.ForEach(item => item[ctx.Index.Global].SetPlayerName(ctx.Name));
        _huds.ForEach(item => item[ctx.Index.Global].SetPlayerNumber(ctx.Index));
        _huds.ForEach(item => item[ctx.Index.Global].SetPlayerIconIndex(ctx.Index));
    }

    public void Destroy(QuantumGame game, PlayerInfoCallbackContext ctx)
    {
        _huds.ForEach(item => item[ctx.Index.Global].gameObject.SetActive(false));
    }

    private void Awake()
    {
        _instance = this;

        QuantumEvent.Subscribe<EventOnPlayerSetName>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].SetPlayerName(e.Name)));
        QuantumEvent.Subscribe<EventOnPlayerSetIcon>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].SetPlayerIconIndex(e.Index)));
        QuantumEvent.Subscribe<EventOnPlayerReady>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].UpdateReadiness(true)));
        QuantumEvent.Subscribe<EventOnPlayerCancel>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].UpdateReadiness(false)));
        QuantumEvent.Subscribe<EventOnPlayerUpdateReady>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].UpdateReadinessValue(e.Readiness.AsFloat)));
        QuantumEvent.Subscribe<EventOnPlayerModifyHealth>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].UpdateHealth(e.NewHealth, e.MaxHealth)));
        QuantumEvent.Subscribe<EventOnPlayerModifyEnergy>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].UpdateEnergy(e.NewEnergy, e.MaxEnergy)));
        QuantumEvent.Subscribe<EventOnPlayerModifyStocks>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].UpdateStocks(e.NewStocks, e.MaxStocks)));
        QuantumEvent.Subscribe<EventOnHideShowReadiness>(listener: this, handler: (e) => _huds.ForEach(item => item[e.Index.Global].ShowReadiness(e.ShowReadiness && e.Index.Type == FighterType.Human)));

        _huds = FindObjectsByType<HUDContainer>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None).Select(item => item.Links).ToList();
    }

    public void ShowAllReadies(bool doShow)
    {
        foreach (var hud in _huds)
        {
            foreach (var playerHUD in hud)
            {
                playerHUD.ShowReadiness(doShow);
            }
        }
    }

    public void SetEnabled(bool isEnabled)
    {
        if (!_instance)
            return;

        _instance.gameObject.SetActive(isEnabled);
    }
}
