using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Effects/Skew", 79)]
    [ExecuteAlways]
    [RequireComponent(typeof(Graphic), typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class Skew : BaseMeshEffect
    {
        [SerializeField] private Vector3 _skewVector;
        public Vector3 SkewVector => _skewVector;
        public void SetSkewVector(Vector2 skewVector) => _skewVector = skewVector;

        private Graphic _graphic;
        private Graphic Graphic => _graphic = _graphic ? _graphic : GetComponent<Graphic>();

        private TMPro.TMP_Text _text;
        private TMPro.TMP_Text Text => _text = _text ? _text : GetComponent<TMPro.TMP_Text>();

        protected override void Awake()
        {
            base.Awake();

            _graphic = GetComponent<Graphic>();
            _text = GetComponent<TMPro.TMP_Text>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Add(TEXT_CHANGED);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(TEXT_CHANGED);
        }

        private void TEXT_CHANGED(object obj)
        {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Text && (obj == _text))
                ModifyTMP((obj as TMPro.TMP_Text).textInfo);
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
        }

        public void ModifyTMP(TMPro.TMP_TextInfo textInfo)
        {
            TMPro.TMP_MeshInfo meshInfo = textInfo.meshInfo[0];
            int lineOffset = 0;

            for (int i = 0; i < textInfo.lineCount; ++i)
            {
                TMPro.TMP_LineInfo lineInfo = textInfo.lineInfo[i];
                float y = 0;

                for (int j = 0; j < lineInfo.characterCount; ++j)
                {
                    int index = lineOffset + j;
                    TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
                    if (charInfo.character.CompareTo(' ') == 0)
                    {
                        y += _skewVector.y * 10;
                        continue;
                    }

                    float width = charInfo.bottomRight.x - charInfo.bottomLeft.x;

                    int bottomLeft = charInfo.vertexIndex;
                    int topLeft = charInfo.vertexIndex + 1;
                    int topRight = charInfo.vertexIndex + 2;
                    int bottomRight = charInfo.vertexIndex + 3;

                    meshInfo.vertices[bottomLeft] += new Vector3(-_skewVector.x, y);
                    meshInfo.vertices[topLeft] += new Vector3(_skewVector.x, y);

                    y += width * _skewVector.y;

                    meshInfo.vertices[topRight] += new Vector3(_skewVector.x, y);
                    meshInfo.vertices[bottomRight] += new Vector3(-_skewVector.x, y);

                    if (j != lineInfo.characterCount - 1)
                    {
                        TMPro.TMP_CharacterInfo nextCharInfo = textInfo.characterInfo[index + 1];
                        float spacing = nextCharInfo.bottomLeft.x - charInfo.bottomRight.x;

                        y += spacing * _skewVector.y;
                    }
                }

                lineOffset += lineInfo.characterCount;
            }

            textInfo.textComponent.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Vertices);
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (_skewVector == Vector3.zero)
                return;

            Rect rect = _graphic.rectTransform.rect;
            Vector4 vec = new(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);

            Color32 color = _graphic.color;

            vh.Clear();
            vh.AddUIVertexQuad(new UIVertex[]
            {
                new()
                {
                    position = new(vec.x - _skewVector.x, vec.y - _skewVector.y),
                    color = color,
                    uv0 = new(0f, 0f)
                },
                new()
                {
                    position = new(vec.x + _skewVector.x, vec.w - _skewVector.y),
                    color = color,
                    uv0 = new(0f, 1f)
                },
                new()
                {
                    position = new(vec.z + _skewVector.x, vec.w + _skewVector.y),
                    color = color,
                    uv0 = new(1f, 1f)
                },
                new()
                {
                    position = new(vec.z - _skewVector.x, vec.y + _skewVector.y),
                    color = color,
                    uv0 = new(1f, 0f)
                }
            });

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

#if UNITY_EDTIOR
        protected override void OnValidate() => UpdateMesh();
#endif

        public void UpdateMesh()
        {
            if (Graphic)
                _graphic.SetVerticesDirty();

            if (Text)
                _text.ForceMeshUpdate();
        }
    }
}
