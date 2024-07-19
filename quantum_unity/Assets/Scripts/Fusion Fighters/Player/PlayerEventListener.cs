using GameResources;
using Quantum;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerEventListener : MonoBehaviour
{
    [SerializeField] private VFX _jump;
    [SerializeField] private VFX _doubleJump;

    private EntityViewUpdater _entityViewUpdater;

    private readonly Dictionary<EntityRef, SkinnedMeshRenderer[]> _meshRenderers = new();
    private readonly Dictionary<EntityRef, SkinnedMeshRenderer> _heads = new();
    private readonly Dictionary<EntityRef, HurtboxSettings> _hurtboxSettings = new();

    [SerializeField] private float _pingPongSpeed;
    [SerializeField] private List<Extensions.Types.Tuple<string, float>> _default;

    [SerializeField] private List<Extensions.Types.Tuple<string, Extensions.Types.Tuple<float, float>>> _invincible;
    [SerializeField] private List<Extensions.Types.Tuple<string, Extensions.Types.Tuple<float, float>>> _intangible;
    [SerializeField] private List<Extensions.Types.Tuple<string, Extensions.Types.Tuple<float, float>>> _superArmor;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnPlayerAttack>(listener: this, handler: e =>
        {
            SetPlayerExpressionYell(e.Player, 1);

            Extensions.Miscellaneous.Helper.Delay(1, () => SetPlayerExpressionNeutral(e.Player, 1));
        });
        QuantumEvent.Subscribe<EventOnPlayerBlockHit>(listener: this, handler: e =>
        {
            SetPlayerExpressionFocused(e.Player, 1);

            Extensions.Miscellaneous.Helper.Delay(1, () => SetPlayerExpressionNeutral(e.Player, 1));
        });
        QuantumEvent.Subscribe<EventOnPlayerHit>(listener: this, handler: e =>
        {
            SetPlayerExpressionShocked(e.Player, 1);

            Extensions.Miscellaneous.Helper.Delay(1, () => SetPlayerExpressionNeutral(e.Player, 1));
        });
        QuantumEvent.Subscribe<EventOnPlayerChangeDirection>(listener: this, handler: ChangeDirection);
        QuantumEvent.Subscribe<EventOnPlayerJump>(listener: this, handler: e =>
        {
            SetPlayerExpressionFocused(e.Player, 1);
            SpawnPlayerJumpVFX(e.Game, e.Player, e.Index.Global);

            Extensions.Miscellaneous.Helper.Delay(1, () => SetPlayerExpressionNeutral(e.Player, 1));
        });
        QuantumEvent.Subscribe<EventOnPlayerJump>(listener: this, handler: e =>
        {
            SetPlayerExpressionFocused(e.Player, 1);
            SpawnPlayerDoubleJumpVFX(e.Game, e.Player, e.Index.Global);

            Extensions.Miscellaneous.Helper.Delay(1, () => SetPlayerExpressionNeutral(e.Player, 1));
        });
        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: StartBlinking);
        QuantumEvent.Subscribe<EventOnPlayerDespawn>(listener: this, handler: StopBlinking);
        QuantumEvent.Subscribe<EventOnHurtboxStateChange>(listener: this, handler: UpdateHurtbox);
    }

    private void Update()
    {
        foreach (var kvp in _hurtboxSettings)
        {
            UpdateHurtboxVisuals(kvp.Key, kvp.Value, Mathf.PingPong(Time.time * _pingPongSpeed, 1));
        }
    }
    
    public void StartBlinking(EventOnPlayerSpawn e)
    {
        StartCoroutine(Blink(e.Player));
    }

    public void StopBlinking(EventOnPlayerDespawn e)
    {
        StopCoroutine(Blink(e.Player));
    }

    private IEnumerator Blink(EntityRef player)
    {
        for (int i = 0; i <= 10; ++i)
        {
            Blink(player, i * (1.0f / 10));
            yield return new WaitForEndOfFrame();
        }

        for (int i = 10; i >= 0; --i)
        {
            Blink(player, i * (1.0f / 10));
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(Random.Range(1f, 4f));
        yield return Blink(player);
    }

    private void UpdateHurtboxVisuals(EntityRef owner, HurtboxSettings settings, float lerpValue)
    {
        foreach (SkinnedMeshRenderer renderer in _meshRenderers[owner])
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; ++i)
            {
                if (!settings.CanBeInterrupted && !settings.CanBeDamaged && !settings.CanBeKnockedBack)
                {
                    for (int j = 0; j < _intangible.Count; ++j)
                    {
                        materials[i].SetFloat(_intangible[j].Item1, Mathf.Lerp(_intangible[j].Item2.Item1, _intangible[j].Item2.Item2, lerpValue));
                    }
                }
                else if (!settings.CanBeInterrupted && !settings.CanBeDamaged && settings.CanBeKnockedBack)
                {
                    for (int j = 0; j < _invincible.Count; ++j)
                    {
                        materials[i].SetFloat(_invincible[j].Item1, Mathf.Lerp(_invincible[j].Item2.Item1, _invincible[j].Item2.Item2, lerpValue));
                    }
                }
                else if (!settings.CanBeInterrupted && settings.CanBeDamaged && !settings.CanBeKnockedBack)
                {
                    for (int j = 0; j < _superArmor.Count; ++j)
                    {
                        materials[i].SetFloat(_superArmor[j].Item1, Mathf.Lerp(_superArmor[j].Item2.Item1, _superArmor[j].Item2.Item2, lerpValue));
                    }
                }
                else
                {
                    for (int j = 0; j < _default.Count; ++j)
                    {
                        materials[i].SetFloat(_default[j].Item1, _default[j].Item2);
                    }
                }
            }
        }
    }

    private void UpdateHurtbox(EventOnHurtboxStateChange e)
    {
        InitList(e.Owner);

        if (!_hurtboxSettings.ContainsKey(e.Owner))
            _hurtboxSettings.Add(e.Owner, default);

        _hurtboxSettings[e.Owner] = e.Settings;
    }

    private void ChangeDirection(EventOnPlayerChangeDirection e)
    {
        _entityViewUpdater.GetView(e.Player).transform.localScale = new(e.Direction, 1, 1);
    }

    public void SpawnPlayerJumpVFX(QuantumGame game, EntityRef player, int index)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_jump);

        if (effect)
            effect.transform.position = _entityViewUpdater.GetView(player).transform.position;
    }

    public void SpawnPlayerDoubleJumpVFX(QuantumGame game, EntityRef player, int index)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_doubleJump);

        if (effect)
            effect.transform.position = _entityViewUpdater.GetView(player).transform.position;
    }

    public void SetPlayerExpressionAngry(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(0, weight * 0.25f);
        head.SetBlendShapeWeight(1, weight * 0.25f);
        head.SetBlendShapeWeight(6, weight);
        head.SetBlendShapeWeight(7, weight);
        head.SetBlendShapeWeight(11, weight);
    }

    public void SetPlayerExpressionYell(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(0, weight * 0.25f);
        head.SetBlendShapeWeight(1, weight * 0.25f);
        head.SetBlendShapeWeight(6, weight);
        head.SetBlendShapeWeight(7, weight);
        head.SetBlendShapeWeight(11, weight);
        head.SetBlendShapeWeight(4, weight);
    }

    public void SetPlayerExpressionSad(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(8, weight);
        head.SetBlendShapeWeight(9, weight);
        head.SetBlendShapeWeight(10, weight);
    }

    public void SetPlayerExpressionScared(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(4, weight * 0.75f);
        head.SetBlendShapeWeight(6, weight);
        head.SetBlendShapeWeight(7, weight);
        head.SetBlendShapeWeight(8, weight);
        head.SetBlendShapeWeight(9, weight);
        head.SetBlendShapeWeight(10, weight);
        head.SetBlendShapeWeight(14, weight * 0.5f);
        head.SetBlendShapeWeight(15, weight * 0.5f);
    }

    public void SetPlayerExpressionHappy(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(3, weight);
        head.SetBlendShapeWeight(13, weight * 0.5f);
    }

    public void SetPlayerExpressionFocused(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(0, weight * 0.4f);
        head.SetBlendShapeWeight(1, weight * 0.4f);
        head.SetBlendShapeWeight(2, weight * 0.4f);
        head.SetBlendShapeWeight(6, weight);
        head.SetBlendShapeWeight(7, weight);
        head.SetBlendShapeWeight(8, weight);
        head.SetBlendShapeWeight(9, weight);
    }

    public void SetPlayerExpressionShocked(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(4, weight);
        head.SetBlendShapeWeight(10, weight);
        head.SetBlendShapeWeight(14, weight * 0.4f);
        head.SetBlendShapeWeight(15, weight * 0.4f);
    }

    public void SetPlayerExpressionNeutral(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        head.SetBlendShapeWeight(0, weight * 0.2f);
        head.SetBlendShapeWeight(1, weight * 0.2f);
        head.SetBlendShapeWeight(2, 0);
        head.SetBlendShapeWeight(3, 0);
        head.SetBlendShapeWeight(4, 0);
        head.SetBlendShapeWeight(5, 0);
        head.SetBlendShapeWeight(6, 0);
        head.SetBlendShapeWeight(7, 0);
        head.SetBlendShapeWeight(8, 0);
        head.SetBlendShapeWeight(9, 0);
        head.SetBlendShapeWeight(10, 0);
        head.SetBlendShapeWeight(11, 0);
        head.SetBlendShapeWeight(12, 0);
        head.SetBlendShapeWeight(13, 0);
        head.SetBlendShapeWeight(14, 0);
        head.SetBlendShapeWeight(15, 0);
    }

    public void Blink(EntityRef owner, float weight)
    {
        SkinnedMeshRenderer head = GetHead(owner);

        if (head)
        {
            head.SetBlendShapeWeight(0, Mathf.Lerp(20, 100, weight));
            head.SetBlendShapeWeight(1, Mathf.Lerp(20, 100, weight));
        }
    }

    private void InitList(EntityRef owner)
    {
        if (!_meshRenderers.ContainsKey(owner))
        {
            _meshRenderers.Add(owner, _entityViewUpdater.GetView(owner).GetComponentsInChildren<SkinnedMeshRenderer>());
            _heads.Add(owner, _meshRenderers[owner].First(item => item.name == "OBJ_head"));
        }
    }

    private SkinnedMeshRenderer GetHead(EntityRef owner)
    {
        InitList(owner);
        return _heads[owner];
    }
}
