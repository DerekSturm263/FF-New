using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("Layout/Track Layout Group", 150)]
#if false
    public class TrackLayoutGroup : MonoBehaviour
    {
        [SerializeField] private List<Vector3> _positions;
        public ref List<Vector3> Positions => ref _positions;

        private List<RectTransform> _rects;

        private void Awake()
        {
            _rects = new();

            for (int i = 0; i < transform.childCount; ++i)
                _rects.Add(transform.GetChild(i).GetComponent<RectTransform>());
        }

        private void Update()
        {
            for (int i = 0; i < _rects.Count; ++i)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), _positions[i], Camera.main, out Vector2 position))
                    _rects[i].anchoredPosition = Camera.main.WorldToScreenPoint(position);
            }
        }
    }
#else
    public class TrackLayoutGroup : VerticalLayoutGroup
    {

    }
#endif
}
