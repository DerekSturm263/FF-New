using UnityEngine;

namespace Extensions.Components.UI
{
    public class TMPLetterByLetterTextEffect : TMPTextEffect
    {
        [SerializeField] private AnimationCurve _curve;

        [SerializeField] private float _speed = 1;
        [SerializeField] private float _spacing = 0.1f;

        protected override void ModifyTextMesh(TMPro.TMP_TextInfo textInfo, TMPro.TMP_MeshInfo meshInfo, float deltaTime, float time)
        {
            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.character.CompareTo(' ') == 0)
                    continue;

                byte alpha = (byte)(_curve.Evaluate((time * _speed) - (i * _spacing)) * 255);

                int bottomLeft = charInfo.vertexIndex;
                int topLeft = charInfo.vertexIndex + 1;
                int topRight = charInfo.vertexIndex + 2;
                int bottomRight = charInfo.vertexIndex + 3;

                meshInfo.colors32[bottomLeft] = new(meshInfo.colors32[bottomLeft].r, meshInfo.colors32[bottomLeft].g, meshInfo.colors32[bottomLeft].b, alpha);
                meshInfo.colors32[topLeft] = new(meshInfo.colors32[topLeft].r, meshInfo.colors32[topLeft].g, meshInfo.colors32[topLeft].b, alpha);

                meshInfo.colors32[topRight] = new(meshInfo.colors32[topRight].r, meshInfo.colors32[topRight].g, meshInfo.colors32[topRight].b, alpha);
                meshInfo.colors32[bottomRight] = new(meshInfo.colors32[bottomRight].r, meshInfo.colors32[bottomRight].g, meshInfo.colors32[bottomRight].b, alpha);
            }
        }
    }
}
