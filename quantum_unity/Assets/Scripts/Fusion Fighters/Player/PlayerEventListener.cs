using GameResources;
using Quantum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerAttack;
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerBlockHit;
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerChangeDirection;
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerJump;
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerDoubleJump;
    [SerializeField] private UnityEvent<QuantumGame, PlayerLink> _onPlayerSpawn;

    [SerializeField] private VFX _jump;
    [SerializeField] private VFX _doubleJump;

    private EntityViewUpdater _entityViewUpdater;
    private readonly Dictionary<EntityRef, SkinnedMeshRenderer> _meshRenderers = new();

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnPlayerAttack>(listener: this, handler: e => _onPlayerAttack.Invoke(e.Game, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerBlockHit>(listener: this, handler: e => _onPlayerBlockHit.Invoke(e.Game, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerChangeDirection>(listener: this, handler: e => _onPlayerChangeDirection.Invoke(e.Game, e.Player));
        QuantumEvent.Subscribe<EventOnPlayerJump>(listener: this, handler: e =>
        {
            if (e.Count == 0)
                _onPlayerJump.Invoke(e.Game, e.Player);
            else
                _onPlayerDoubleJump.Invoke(e.Game, e.Player);
        });
        QuantumEvent.Subscribe<EventOnPlayerSpawn>(listener: this, handler: e => _onPlayerSpawn.Invoke(e.Game, e.Player));

        QuantumEvent.Subscribe<EventOnHurtboxStateChange>(listener: this, handler: UpdateHurtbox);
    }

    private void UpdateHurtbox(EventOnHurtboxStateChange e)
    {
        if (!_meshRenderers.ContainsKey(e.Owner))
            _meshRenderers.Add(e.Owner, _entityViewUpdater.GetView(e.Owner).GetComponentInChildren<SkinnedMeshRenderer>());

        Material[] materials = _meshRenderers[e.Owner].materials;

        for (int i = 0; i < materials.Length; ++i)
        {
            if (!e.Settings.CanBeDamaged)
            {
                materials[i].SetFloat("_Emission_Weight", 0.075f);
                materials[i].SetFloat("_Fresnal_Strength", 1);
            }
            else
            {
                materials[i].SetFloat("_Emission_Weight", 0);
                materials[i].SetFloat("_Fresnal_Strength", 0.2f);
            }
        }
    }

    public void SpawnPlayerJumpVFX(QuantumGame game, PlayerLink player)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_jump);
        effect.transform.position = _entityViewUpdater.GetView(player.Entity).transform.position;
    }

    public void SpawnPlayerDoubleJumpVFX(QuantumGame game, PlayerLink player)
    {
        GameObject effect = VFXController.Instance.SpawnEffect(_doubleJump);
        effect.transform.position = _entityViewUpdater.GetView(player.Entity).transform.position;
    }
}
