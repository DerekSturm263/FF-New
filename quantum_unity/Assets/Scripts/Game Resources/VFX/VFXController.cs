using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace GameResources
{
    public class VFXController : Extensions.Components.Miscellaneous.Controller<VFXController>
    {
        public GameObject SpawnEffectParented(VFX settings, Transform parent)
        {
            GameObject effect;

            if (settings.DoesFollowParent)
            {
                effect = Instantiate(settings.VFXObject, parent);
            }
            else
            {
                effect = Instantiate(settings.VFXObject, parent.transform.position, Quaternion.identity);
            }

            effect.transform.right = settings.Direction.Normalized.ToUnityVector3();
            effect.transform.localScale *= settings.ScaleMultiplier.ToUnityVector2();
            effect.transform.localPosition += settings.Offset.ToUnityVector3();

            return effect;
        }

        public GameObject SpawnEffect(VFX settings)
        {
            GameObject effect = Instantiate(settings.VFXObject, settings.Offset.ToUnityVector3(), Quaternion.identity);

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
