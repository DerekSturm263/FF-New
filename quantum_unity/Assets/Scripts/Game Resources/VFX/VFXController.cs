using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace GameResources
{
    public class VFXController : Extensions.Components.Miscellaneous.Controller<VFXController>
    {
        private bool _isEnabled = true;
        public bool IsEnabled => _isEnabled;
        public void SetIsEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;

            foreach (ConditionalEnable component in FindObjectsByType<ConditionalEnable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                component.enabled = _isEnabled;
            }
        }

        public GameObject SpawnEffectParented(VFXSettings settings, Transform parent)
        {
            if (!_isEnabled)
                return null;

            GameObject effect;

            if (settings.DoesFollowParent)
            {
                effect = Instantiate(settings.VFXObject, parent);
            }
            else
            {
                effect = Instantiate(settings.VFXObject, parent.transform.position, Quaternion.identity);
            }

            effect.transform.right = settings.Direction.normalized;
            effect.transform.localScale = Vector3.Scale(effect.transform.localScale, settings.ScaleMultiplier);
            effect.transform.localPosition += settings.Offset;

            Debug.Log($"Spawning VFX {settings.VFXObject.name}");

            return effect;
        }

        public GameObject SpawnEffectUnparented(VFXSettings settings)
        {
            if (!_isEnabled)
                return null;

            GameObject effect = Instantiate(settings.VFXObject, settings.Offset, Quaternion.identity);

            effect.transform.right = settings.Direction.normalized;
            effect.transform.localScale = Vector3.Scale(effect.transform.localScale, settings.ScaleMultiplier);

            Debug.Log($"Spawning VFX {settings.VFXObject.name}");

            return effect;
        }

        public GameObject SpawnEffect(VFX settings)
        {
            if (!_isEnabled)
                return null;

            GameObject effect = Instantiate(settings.Settings.VFXObject, settings.Settings.Offset, Quaternion.identity);

            effect.transform.right = settings.Settings.Direction.normalized;
            effect.transform.localScale = Vector3.Scale(effect.transform.localScale, settings.Settings.ScaleMultiplier);
            effect.transform.localPosition += settings.Settings.Offset;

            Debug.Log($"Spawning VFX {settings.Settings.VFXObject.name}");

            return effect;
        }

        public void SpawnItemVFX(QuantumGame game, EntityView user, (EntityView itemObj, ItemAsset itemAsset, FPVector2 position) tuple)
        {
            if (tuple.itemAsset.ItemVFX)
            {
                GameObject effect = SpawnEffect(tuple.itemAsset.ItemVFX);

                if (effect)
                    effect.transform.position = tuple.position.XYO.ToUnityVector3();
            }
            if (tuple.itemAsset.UserVFX)
            {
                SpawnEffectParented(tuple.itemAsset.UserVFX.Settings, user.transform);
            }
        }
    }
}
