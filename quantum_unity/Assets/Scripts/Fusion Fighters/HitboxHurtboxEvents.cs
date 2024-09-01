using GameResources;
using Quantum;
using UnityEngine;

public class HitboxHurtboxEvents : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffectCircle;
    [SerializeField] private Vector3 _scale;
    [SerializeField] private GameObject _hitEffectSpark;
    [SerializeField] private Vector3 _scale2;

    private EntityViewUpdater _viewUpdater;

    private void Awake()
    {
        _viewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnHitboxHurtboxCollision>(listener: this, handler: HandleHitboxHurtboxCollision);
        QuantumEvent.Subscribe<EventOnHitboxSpawnDespawn>(listener: this, handler: HandleHitboxSpawnDespawn);
    }

    private void HandleHitboxHurtboxCollision(EventOnHitboxHurtboxCollision e)
    {
        if (!e.DefenderIndex.Equals(FighterIndex.Invalid) && PlayerJoinController.Instance.TryGetPlayer(e.DefenderIndex, out LocalPlayerInfo player))
        {
            ShakeSettings shake = e.Game.Frames.Verified.FindAsset<ShakeSettings>(e.Settings.Visual.TargetShake.Id);
            PlayerJoinController.Instance.Rumble(player, player.Profile.value.HapticStrength * shake.Strength.AsFloat * 0.1f, 0.3f);
        }

        VFXController.Instance.SpawnEffectUnparented(new()
        {
            Parent = VFXSettings.ParentType.None,
            DoesFollowParent = false,
            Direction = e.Direction.ToUnityVector3(),
            Offset = e.Position.ToUnityVector3(),
            VFXObject = _hitEffectCircle,
            ScaleMultiplier = _scale
        });

        /*GameObject sparks = VFXController.Instance.SpawnEffectUnparented(new()
        {
            Parent = VFXSettings.ParentType.None,
            DoesFollowParent = false,
            Direction = e.Direction.ToUnityVector3(),
            Offset = e.Position.ToUnityVector3(),
            VFXObject = _hitEffectSpark,
            ScaleMultiplier = _scale2
        });

        for (int i = 0; i < sparks.transform.childCount; ++i)
        {
            ParticleSystem child = sparks.transform.GetChild(i).GetComponent<ParticleSystem>();
            //child.
        }*/
    }

    private void HandleHitboxSpawnDespawn(EventOnHitboxSpawnDespawn e)
    {
        TrailRenderer trailRenderer = _viewUpdater.GetView(e.Owner).gameObject.GetComponentInChildren<TrailRenderer>();

        if (trailRenderer)
            trailRenderer.emitting = e.IsEnabled;
    }
}
