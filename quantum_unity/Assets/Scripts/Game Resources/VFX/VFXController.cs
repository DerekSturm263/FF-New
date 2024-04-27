using UnityEngine;

namespace GameResources
{
    public class VFXController : Extensions.Components.Miscellaneous.Controller<VFXController>
    {
        private Transform _parent;

        public void SetParent(Transform parent)
        {
            _parent = parent;
        }

        public void SpawnEffect(VFX settings)
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
        }
    }
}
