﻿using Extensions.Components.Miscellaneous;
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
        [SerializeField] private EntityViewUpdater _entityView;
        [SerializeField] private List<Tuple<float, Transform>> _targets;

        [SerializeField] private CameraSettingsAsset _settings;
        public void SetCameraSettings(CameraSettingsAsset settings) => _instance._settings = settings;
        public void SetCameraSettingsFromStageDefault(Stage stage)
        {
            _instance._settings = UnityDB.FindAsset<CameraSettingsAsset>(stage.Theme.CameraSettings.Default.Id);
        }

        public unsafe void SetCameraSettingsFromCurrentStageDefault()
        {
            AssetGuid guid = QuantumRunner.Default.Game.Frames.Verified.Global->CurrentMatch.Stage.Theme.CameraSettings.Default.Id;
            _instance._settings = UnityDB.FindAsset<CameraSettingsAsset>(guid);
        }
        public unsafe void SetCameraSettingsFromCurrentStageZoom()
        {
            AssetGuid guid = QuantumRunner.Default.Game.Frames.Verified.Global->CurrentMatch.Stage.Theme.CameraSettings.Zoom.Id;
            _instance._settings = UnityDB.FindAsset<CameraSettingsAsset>(guid);
        }
        public unsafe void SetCameraSettingsFromCurrentStageTension()
        {
            AssetGuid guid = QuantumRunner.Default.Game.Frames.Verified.Global->CurrentMatch.Stage.Theme.CameraSettings.Tension.Id;
            _instance._settings = UnityDB.FindAsset<CameraSettingsAsset>(guid);
        }

        public void SetVolume(VolumeProfile volumeProfile) => _instance._volume.sharedProfile = volumeProfile;

        private Volume _volume;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private float _calculatedZoom;
        private float _targetZoom;

        private Vector2 _shakeDirection;
        private Vector3 _shakeAmount;
        private float _shakeTime;

        private UnityEngine.Camera _cam;
        public UnityEngine.Camera Cam => _cam;

        private UnityEngine.Camera[] _internalCams;

        private void Awake()
        {
            Initialize();

            _targetPosition = transform.position;
            _targetRotation = transform.rotation;

            _cam = GetComponent<UnityEngine.Camera>();
            _volume = GetComponent<Volume>();

            _internalCams = GetComponentsInChildren<UnityEngine.Camera>();
        }

        private void LateUpdate()
        {
            if (!_settings)
                return;

            CalculateTargetZoom();
            CalculateTargetPosition();
            CalculateTargetRotation();
            CalculateShake();

            foreach (UnityEngine.Camera cam in _internalCams)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _settings.Settings.FOV, Time.deltaTime * _settings.Settings.TranslationSpeed.AsFloat);
            }

            ApplyPosition(Time.deltaTime);
            ApplyRotation(Time.deltaTime);

            if (_shakeTime > 0)
                _shakeTime -= Time.deltaTime;
        }

        private void CalculateTargetZoom()
        {
            float zoom = _settings.Settings.ZoomOffset.AsFloat;
            _calculatedZoom = zoom;

            if (_targets.Count == 0)
                return;

            Vector3 firstPos = _targets[0].Item2.position;
            Vector3 lastPos = _targets[^1].Item2.position;

            float distance = Vector3.Distance(firstPos, lastPos);
            _calculatedZoom = zoom + distance * _settings.Settings.ZoomScale.AsFloat;
            _targetZoom = Mathf.Lerp(_targetZoom, _calculatedZoom, Time.deltaTime * _settings.Settings.ZoomSpeed.AsFloat);
        }

        private void CalculateTargetPosition()
        {
            Vector3 targetPosition = default;
            
            if (_targets.Count > 0)
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

            _targetPosition = targetPosition + _settings.Settings.TranslationOffset.ToUnityVector3() + _shakeAmount + new Vector3(0, 0, -_targetZoom);
        }

        private void CalculateTargetRotation()
        {
            _targetRotation = Quaternion.Euler(_settings.Settings.RotationOffset.ToUnityVector3());
        }

        private void CalculateShake()
        {
            if (_shakeTime > 0)
                _shakeAmount = (Vector3)_shakeDirection * (float)(_settings.Settings.ShakeCurve.Evaluate(_shakeTime.ToFP() * _settings.Settings.ShakeFrequency) * _settings.Settings.ShakeStrength);
            else
                _shakeAmount = default;
        }

        private void ApplyPosition(float dt)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, dt * _settings.Settings.TranslationSpeed.AsFloat);
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

        public void FocusTarget(int index)
        {
            if (index >= _instance._targets.Count)
                return;

            for (int i = 0; i < _targets.Count; ++i)
            {
                if (i != index)
                    _instance._targets[i] = new(0, _instance._targets[i].Item2);
                else
                    _instance._targets[i] = new(1, _instance._targets[i].Item2);
            }
        }

        public void ResetAllWeights()
        {
            for (int i = 0; i < _targets.Count; ++i)
            {
                _instance._targets[i] = new(1, _instance._targets[i].Item2);
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

        public void Shake(Vector2 amount, float time)
        {
            _instance._shakeDirection = amount;
            _instance._shakeTime = time;
            _instance._shakeAmount = default;
        }

        public void ShakeRandom(float strength)
        {
            Shake(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * strength, 1);
        }
    }
}
