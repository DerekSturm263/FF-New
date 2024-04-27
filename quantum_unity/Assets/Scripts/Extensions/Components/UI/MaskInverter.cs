using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Mask Inverter", 13)]
    [ExecuteAlways]
    [SelectionBase]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [System.Obsolete("This only exists until I can find out a way to make an \"Inverse Mask\" Component.")]
    // TODO: REPLACE WITH INVERSE MASK
    public class MaskInverter : UIBehaviour, IMaterialModifier
    {
        private static readonly int _stencilComp = Shader.PropertyToID("_StencilComp");

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            var resultMaterial = new Material(baseMaterial);
            resultMaterial.SetFloat(_stencilComp, System.Convert.ToSingle(CompareFunction.NotEqual));
            return resultMaterial;
        }
    }
}