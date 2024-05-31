using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extensions.Components.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(TMPro.TMP_Text), typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class TMPTextEffect : UIBehaviour
    {
        [SerializeField] private bool _resetOnTextChange;

        private TMPro.TMP_Text _text;
        private TMPro.TMP_Text Text => _text = _text ? _text : GetComponent<TMPro.TMP_Text>();

        private float _time;
        protected List<Vector3> _allVertices = new();

        protected override void Awake()
        {
            base.Awake();

            _text = GetComponent<TMPro.TMP_Text>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SaveAllVertices(Text.textInfo);
            _text.ForceMeshUpdate();
            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Add(TEXT_CHANGED);

            _time = 0;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(TEXT_CHANGED);
            _text.ForceMeshUpdate();
            
            _time = 0;
        }

        private void TEXT_CHANGED(object obj)
        {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Text && (obj == _text))
            {
                SaveAllVertices((obj as TMPro.TMP_Text).textInfo);

                if (_resetOnTextChange)
                    _time = 0;
            }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
        }

        private void SaveAllVertices(TMPro.TMP_TextInfo textInfo)
        {
            if (textInfo.meshInfo[0].vertices is null)
                return;

            _allVertices.Clear();
            _allVertices.AddRange(textInfo.meshInfo[0].vertices);
        }

        private void FixedUpdate()
        {
            ModifyTMP(Text.textInfo, Time.fixedDeltaTime);
        }

        public void ModifyTMP(TMPro.TMP_TextInfo textInfo, float deltaTime)
        {
            TMPro.TMP_MeshInfo meshInfo = textInfo.meshInfo[0];

            textInfo.textComponent.ClearMesh();

            ModifyTextMesh(textInfo, meshInfo, deltaTime, _time);

            textInfo.textComponent.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Vertices | TMPro.TMP_VertexDataUpdateFlags.Colors32);

            _time += deltaTime;
        }

        protected abstract void ModifyTextMesh(TMPro.TMP_TextInfo textInfo, TMPro.TMP_MeshInfo meshInfo, float deltaTime, float time);

#if UNITY_EDTIOR
        protected override void OnValidate() => UpdateMesh();
#endif

        public void UpdateMesh()
        {
            if (Text)
                _text.ForceMeshUpdate();
        }
    }
}
