using Extensions.Components.Miscellaneous;
using Extensions.Miscellaneous;
using Extensions.Types;
using Photon.Deterministic;
using Quantum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameResources.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraController : Controller<CameraController>
    {
        [SerializeField] private List<AssetRefCameraSettings> _uninteruptables;

        [SerializeField] private List<Tuple<float, Transform>> _targets;

        [SerializeField] private CameraSettingsAsset _settings;
        public void SetCameraSettings(CameraSettingsAsset settings)
        {
            if (!_instance || _uninteruptables.Any(item => item.Id == _settings.Settings.Guid))
                return;

            _instance._settings = settings;

            if (settings.Volume)
                _instance.SetVolumeInternal(settings.Volume);
        }
        public void SetCameraSettingsWithoutNotify(CameraSettingsAsset settings) => _settings = settings;

        [SerializeField] private bool _isMain = true;

        private void SetVolumeInternal(VolumeProfile volumeProfile) => _volume.sharedProfile = volumeProfile;

        public void SetCameraSettingsFromStageDefault(Stage stage)
        {
            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(stage.Theme.CameraSettings.Default.Id));
        }

        public void SetCameraSettingsFromStageZoom(Stage stage)
        {
            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(stage.Theme.CameraSettings.Zoom.Id));
        }

        public void SetCameraSettingsFromStageTension(Stage stage)
        {
            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(stage.Theme.CameraSettings.Tension.Id));
        }

        public void SetCameraSettingsFromStageWinner(Stage stage)
        {
            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(stage.Theme.CameraSettings.Winner.Id));
        }

        public unsafe void SetCameraSettingsFromCurrentStageDefault()
        {
            Stage stage = QuantumRunner.Default.Game.Frames.Verified.Global->CurrentMatch.Stage;
            AssetGuid guid = stage.Theme.CameraSettings.Default.Id;

            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(guid));
        }

        public unsafe void SetCameraSettingsFromCurrentStageZoom()
        {
            Stage stage = QuantumRunner.Default.Game.Frames.Verified.Global->CurrentMatch.Stage;
            AssetGuid guid = stage.Theme.CameraSettings.Zoom.Id;

            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(guid));
        }

        public unsafe void SetCameraSettingsFromCurrentStageTension()
        {
            Stage stage = QuantumRunner.Default.Game.Frames.Verified.Global->CurrentMatch.Stage;
            AssetGuid guid = stage.Theme.CameraSettings.Tension.Id;

            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(guid));
        }

        public unsafe void SetCameraSettingsFromCurrentStageWinner()
        {
            Stage stage = QuantumRunner.Default.Game.Frames.Verified.Global->CurrentMatch.Stage;
            AssetGuid guid = stage.Theme.CameraSettings.Winner.Id;

            SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(guid));
        }

        public void StartParticles()
        {
            _instance._particleSystem.Play();
        }

        private EntityViewUpdater _entityView;
        private ParticleSystem _particleSystem;
        private Volume _volume;
        private ManualCameraController _manualMode;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private float _calculatedZoom;
        private float _targetZoom;

        private ShakeSettings _shakeSettings;
        private Vector2 _shakeDirection;

        private float _shakeTime;
        private Vector2 _shakeAmount;

        private UnityEngine.Camera _cam;
        public UnityEngine.Camera Cam => _cam;

        private UnityEngine.Camera[] _internalCams;

        private void Awake()
        {
            if (_isMain)
            {
                Initialize();

                QuantumEvent.Subscribe<EventOnCameraShake>(listener: this, handler: e => Shake(e.Settings, e.Direction.ToUnityVector2(), e.Global, e.Defender));
                QuantumEvent.Subscribe<EventOnSetCameraSettings>(listener: this, handler: e =>
                {
                    var oldSettings = _settings;
                    //SetCameraSettings(UnityDB.FindAsset<CameraSettingsAsset>(e.Settings.Id));

                    //Helper.Delay(e.Time.AsFloat, () => SetCameraSettings(oldSettings));
                });
            }

            _targetPosition = transform.position;
            _targetRotation = transform.rotation;

            _cam = GetComponent<UnityEngine.Camera>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _volume = GetComponent<Volume>();
            _manualMode = GetComponent<ManualCameraController>();

            _entityView = FindFirstObjectByType<EntityViewUpdater>();
            _internalCams = GetComponentsInChildren<UnityEngine.Camera>(true);
        }

        private void LateUpdate()
        {
            if (!_settings)
                return;

            float dt = Time.timeScale == 0 ? Time.unscaledDeltaTime : Time.deltaTime;

            CalculateTargetZoom(dt);
            CalculateTargetPosition();
            CalculateTargetRotation();
            CalculateShake();

            foreach (UnityEngine.Camera cam in _internalCams)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _settings.Settings.FOV, dt * _settings.Settings.TranslationSpeed.AsFloat);
            }

            ApplyPosition(dt);
            ApplyRotation(dt);

            if (_shakeTime > 0)
                _shakeTime -= dt;
        }

        private void CalculateTargetZoom(float dt)
        {
            float zoom = _settings.Settings.ZoomOffset.AsFloat;
            _calculatedZoom = zoom;

            if (_targets.Count == 0)
                return;

            Vector3 firstPos = _targets[0].Item2.position;
            Vector3 lastPos = _targets[^1].Item2.position;

            float distance = Vector3.Distance(firstPos, lastPos);
            _calculatedZoom = zoom + distance * _settings.Settings.ZoomScale.AsFloat;
            _targetZoom = Mathf.Lerp(_targetZoom, _calculatedZoom, dt * _settings.Settings.ZoomSpeed.AsFloat);
        }

        private void CalculateTargetPosition()
        {
            Vector3 targetPosition = default;
            
            if (_targets.Count > 0 && _targets.Sum(item => item.Item1) > 0)
            {
                float targetCountWeight = 0;
                for (int i = 0; i < _targets.Count; ++i)
                {
                    targetPosition += _targets[i].Item2.position * _targets[i].Item1;
                    targetCountWeight += _targets[i].Item1;
                }

                targetPosition /= targetCountWeight;
            }
            else
            {
                targetPosition = FPVector3.Lerp(_settings.Settings.TranslationMinClamp, _settings.Settings.TranslationMaxClamp, FP._0_50).ToUnityVector3();
            }

            targetPosition.x = Mathf.Clamp(targetPosition.x, _settings.Settings.TranslationMinClamp.X.AsFloat, _settings.Settings.TranslationMaxClamp.X.AsFloat);
            targetPosition.y = Mathf.Clamp(targetPosition.y, _settings.Settings.TranslationMinClamp.Y.AsFloat, _settings.Settings.TranslationMaxClamp.Y.AsFloat);
            targetPosition.z = Mathf.Clamp(targetPosition.z, _settings.Settings.TranslationMinClamp.Z.AsFloat, _settings.Settings.TranslationMaxClamp.Z.AsFloat);

            _targetPosition = targetPosition + _settings.Settings.TranslationOffset.ToUnityVector3() + new Vector3(0, 0, -_targetZoom);
        }

        private void CalculateTargetRotation()
        {
            _targetRotation = Quaternion.Euler(_settings.Settings.RotationOffset.ToUnityVector3());
        }

        private void CalculateShake()
        {
            if (_shakeTime > 0)
            {
                float x = (_shakeSettings.RelativeHorizontalCurve.Evaluate((_shakeSettings.Length - _shakeTime.ToFP()) * _shakeSettings.Frequency) * _shakeSettings.Strength).AsFloat;
                float y = (_shakeSettings.RelativeVerticalCurve.Evaluate((_shakeSettings.Length - _shakeTime.ToFP()) * _shakeSettings.Frequency) * _shakeSettings.Strength).AsFloat;

                _shakeAmount = (_shakeDirection * y) + (new Vector2(_shakeDirection.y, -_shakeDirection.x) * x);
            }
            else
            {
                _shakeAmount = default;
            }
        }

        private void ApplyPosition(float dt)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, dt * _settings.Settings.TranslationSpeed.AsFloat) + (Vector3)_shakeAmount;
        }

        private void ApplyRotation(float dt)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, dt * _settings.Settings.RotationSpeed.AsFloat);
        }

        public void SetTarget(Transform target)
        {
            _instance._targets.Clear();
            _instance._targets.Add(new(1, target));
        }

        public void SetTarget(string target)
        {
            if (_targets.Count > 0)
                return;

            EntityView entity = _instance._entityView.GetView(FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, item => item.Type == FighterType.Human));
            GameObject head = entity.gameObject.FindChildWithTag(target, false);

            _targets.Clear();
            _targets.Add(new(1, head.transform));
        }

        public void FocusTarget(int globalIndex)
        {
            if (globalIndex >= _instance._targets.Count)
                return;

            for (int i = 0; i < _instance._targets.Count; ++i)
            {
                if (i != globalIndex)
                    _instance._targets[i] = new(0, _instance._targets[i].Item2);
                else
                    _instance._targets[i] = new(1, _instance._targets[i].Item2);
            }
        }

        public void FocusTarget(FighterIndex index)
        {
            EntityRef player = FighterIndex.GetPlayerFromIndex(QuantumRunner.Default.Game.Frames.Verified, index);

            for (int i = 0; i < _instance._targets.Count; ++i)
            {
                if (_instance._targets[i].Item2 == _entityView.GetView(player))
                    _instance._targets[i] = new(0, _instance._targets[i].Item2);
                else
                    _instance._targets[i] = new(1, _instance._targets[i].Item2);
            }
        }

        public void FocusTarget(string name)
        {
            if (!_instance)
                return;

            int index = _instance._targets.FindIndex(item => item.Item2.name == name);
            FocusTarget(index);
        }

        public void ResetAllWeights()
        {
            if (!_instance)
                return;

            for (int i = 0; i < _instance._targets.Count; ++i)
            {
                _instance._targets[i] = new(1, _instance._targets[i].Item2);
            }
        }

        public void ClearAllWeights()
        {
            if (!_instance)
                return;

            for (int i = 0; i < _instance._targets.Count; ++i)
            {
                _instance._targets[i] = new(0, _instance._targets[i].Item2);
            }
        }

        public void SetTargets(IEnumerable<Transform> targets)
        {
            _instance._targets.Clear();
            _instance._targets.AddRange(targets.Select(item => new Tuple<float, Transform>(1, item)));
        }

        public void AddTarget(Transform target)
        {
            _instance._targets.Add(new(1, target));
        }

        public void AddTargetEntity(QuantumGame game, EntityRef player)
        {
            GameObject playerObj = _entityView.GetView(player).gameObject;
            AddTarget(playerObj.transform);
        }

        public void RemoveTarget(Transform target)
        {
            _instance._targets.RemoveAll(item => item.Item2 == target);
        }

        public void ClearTargets()
        {
            _instance._targets.Clear();
        }

        private void Shake(AssetRefShakeSettings settings, Vector2 direction, bool isGlobal, EntityRef defender)
        {
            if (!settings.Id.IsValid)
                return;

            ShakeSettingsAsset shakeSettings = UnityDB.FindAsset<ShakeSettingsAsset>(settings.Id);
            Shake(shakeSettings.Settings, direction);

            if (isGlobal)
            {
                foreach (var player in PlayerJoinController.Instance.GetAllLocalPlayers(true))
                {
                    PlayerJoinController.Instance.Rumble(player, player.Profile.value.HapticStrength * shakeSettings.Settings.Strength.AsFloat * 0.1f, 0.3f);
                }
            }
            else if (QuantumRunner.Default.Game.Frames.Verified.TryGet(defender, out PlayerStats stats))
            {
                if (PlayerJoinController.Instance.TryGetPlayer(stats.Index, out LocalPlayerInfo player))
                    PlayerJoinController.Instance.Rumble(player, player.Profile.value.HapticStrength * shakeSettings.Settings.Strength.AsFloat * 0.1f, 0.3f);
            }
        }

        private void Shake(ShakeSettings settings, Vector2 direction)
        {
            _shakeSettings = settings;

            _instance._shakeTime = _shakeSettings.Length.AsFloat;
            _instance._shakeDirection = direction;
        }

        public void Move(Vector2 input)
        {
            _instance._manualMode.Move(input);
        }

        public void Orbit(Vector2 input)
        {
            _instance._manualMode.Orbit(input);
        }

        public void Zoom(float input)
        {
            _instance._manualMode.Zoom(input);
        }

        public void Tilt(float input)
        {
            _instance._manualMode.Tilt(input);
        }

        public void ToggleManualMode(bool useManual)
        {
            _instance._manualMode.enabled = useManual;
            _instance.enabled = !useManual;
        }

        public void Snap()
        {
            _instance.GetComponent<ManualCameraController>().Snap();
        }
    }
}
