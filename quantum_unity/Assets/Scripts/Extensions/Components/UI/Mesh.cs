using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Effects/Mesh", 80)]
    [ExecuteAlways]
    [SelectionBase]
    [RequireComponent(typeof(Graphic), typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class Mesh : BaseMeshEffect
    {
        private Graphic _graphic;
        private Graphic Graphic => _graphic = _graphic ? _graphic : GetComponent<Graphic>();

        protected override void Awake()
        {
            base.Awake();

            _graphic = GetComponent<Graphic>();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateMesh();
        }
#endif

        public override void ModifyMesh(VertexHelper vh)
        {
            Color32 color = _graphic.color;
            vh.Clear();

            Rect rect = GetComponent<RectTransform>().rect;

            IEnumerable<Vector3> positions = transform.GetChild(0).GetComponentsInChildren<RectTransform>().Select(item => item.localPosition);
            for (int i = 1; i < positions.Count(); ++i)
            {
                Vector2 pos = positions.ElementAt(i);
                float uvX = Mathf.InverseLerp(-rect.width / 2, rect.width / 2, pos.x);
                float uvY = Mathf.InverseLerp(-rect.height / 2, rect.height / 2, pos.y);

                UIVertex uiVertex = new()
                {
                    position = pos,
                    color = color,
                    uv0 = new(uvX, uvY),
                };

                vh.AddVert(uiVertex);
            }

            int index = 0;
            for (int i = 0; i < transform.GetChild(0).childCount - 2; ++i)
            {
                int b = index + 1;
                int c = index + 2;
                vh.AddTriangle(0, b, c);

                index++;
            }
            
            //vh.AddTriangle(0, 1, 2);
            //vh.AddTriangle(2, 3, 0);
        }

#if UNITY_EDTIOR
        protected override void OnValidate() => UpdateMesh();

        [UnityEditor.MenuItem("GameObject/UI/Mesh", priority = 30)]
        private static void Create_Internal(UnityEditor.MenuCommand cmd)
        {
            GameObject obj = new("UI Mesh");

            UnityEditor.GameObjectUtility.SetParentAndAlign(obj, cmd.context as GameObject);
            UnityEditor.Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            UnityEditor.Selection.activeGameObject = obj;

            obj.AddComponent<Mesh>();
        }

#endif

        public void UpdateMesh() => Graphic.SetVerticesDirty();
    }
}
