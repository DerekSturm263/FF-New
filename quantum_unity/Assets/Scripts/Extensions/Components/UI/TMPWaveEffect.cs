using UnityEngine;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Effects/TextMeshPro - Wavy Text", 79)]
    public class TMPWaveEffect : TMPTextEffect
    {
        [SerializeField] private AnimationCurve _curve;

        [SerializeField] private float _strength = 1;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _length = 1;
        [SerializeField] private float _spacing = 0.1f;

        protected override void ModifyTextMesh(TMPro.TMP_TextInfo textInfo, TMPro.TMP_MeshInfo meshInfo, float deltaTime, float time)
        {
            if (_allVertices.Count == 0)
                return;

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.character.CompareTo(' ') == 0)
                    continue;

                float y = _curve.Evaluate(Mathf.Repeat((time + (i * _spacing)) * _speed, _length)) * _strength;

                int bottomLeft = charInfo.vertexIndex;
                int topLeft = charInfo.vertexIndex + 1;
                int topRight = charInfo.vertexIndex + 2;
                int bottomRight = charInfo.vertexIndex + 3;

                meshInfo.vertices[bottomLeft] = _allVertices[bottomLeft] + new Vector3(0, y);
                meshInfo.vertices[topLeft] = _allVertices[topLeft] + new Vector3(0, y);

                meshInfo.vertices[topRight] = _allVertices[topRight] + new Vector3(0, y);
                meshInfo.vertices[bottomRight] = _allVertices[bottomRight] + new Vector3(0, y);
            }
        }
    }
}
