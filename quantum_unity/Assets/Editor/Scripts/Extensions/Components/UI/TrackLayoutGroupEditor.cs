using UnityEditor;
using UnityEngine;
using Extensions.Components.UI;

namespace Editor.Extensions.Components.UI
{
#if false
    [CustomEditor(typeof(TrackLayoutGroup))]
    public class TrackLayoutGroupEditor : Editor
    {
        private void OnSceneGUI()
        {
            TrackLayoutGroup trackLayoutGroup = target as TrackLayoutGroup;

            if (trackLayoutGroup.Positions is null || trackLayoutGroup.Positions.Count == 0)
                return;

            Handles.color = Color.green;

            for (int i = 0; i < trackLayoutGroup.Positions.Count - 1; ++i)
            {
                Handles.DrawLine(trackLayoutGroup.transform.position + trackLayoutGroup.Positions[i], trackLayoutGroup.transform.position + trackLayoutGroup.Positions[i + 1]);
            }

            for (int i = 0; i < trackLayoutGroup.Positions.Count; ++i)
            {
                Vector3 pos = Handles.PositionHandle(trackLayoutGroup.transform.position + trackLayoutGroup.Positions[i], Quaternion.identity);
                trackLayoutGroup.Positions[i] = pos - trackLayoutGroup.transform.position;
            }
        }
    }
#endif
}
