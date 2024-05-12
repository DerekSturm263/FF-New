using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace GameResources
{
    public class VFXController : Extensions.Components.Miscellaneous.Controller<VFXController>
    {
        private Transform _parent;

        public GameObject SpawnEffect(VFX settings)
        {
            GameObject effect = null;
            if (!_parent)
                effect = Instantiate(settings.VFXObject, settings.Offset.ToUnityVector3(), Quaternion.identity);

            if (settings.DoesFollowParent)
            {
                effect = Instantiate(settings.VFXObject, _parent);
            }
            else if (_parent)
            {
                effect = Instantiate(settings.VFXObject, _parent.transform.position, Quaternion.identity);
            }

            effect.transform.right = settings.Direction.Normalized.ToUnityVector3();
            effect.transform.localScale *= settings.ScaleMultiplier.ToUnityVector2();
            effect.transform.localPosition += settings.Offset.ToUnityVector3();

            return effect;
        }

        public void SpawnItemVFX(QuantumGame game, EntityView user, (EntityView itemObj, ItemAsset itemAsset, FPVector2 position) tuple)
        {
            GameObject effect = SpawnEffect(tuple.itemAsset.VFX["Explosion"]);
            effect.transform.position = tuple.position.XYO.ToUnityVector3();
        }
    }
}
