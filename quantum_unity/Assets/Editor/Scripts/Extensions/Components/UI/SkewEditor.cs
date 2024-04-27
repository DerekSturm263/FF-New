using UnityEditor;
using UnityEngine;
using Extensions.Components.UI;

namespace Editor.Extensions.Components.UI
{
    [CustomEditor(typeof(Skew))]
    [CanEditMultipleObjects()]
    public class SkewEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            Skew skew = target as Skew;
            /*
            float x = Handles.RadiusHandle(Quaternion.identity, skew.transform.position + new Vector3(skew.SkewVector.x, 0), 0.1f) - skew.transform.position.x;
            float y = Handles.RadiusHandle(Quaternion.identity, skew.transform.position + new Vector3(0, skew.SkewVector.y), 0.1f) - skew.transform.position.y;

            skew.SetSkewVector(new(x, y));*/

            Vector3 newSkew = Handles.DoPositionHandle(skew.transform.position + skew.SkewVector, Quaternion.identity) - skew.transform.position;
            if (newSkew != skew.SkewVector)
            {
                skew.SetSkewVector(newSkew);
                skew.UpdateMesh();
            }
        }
    }
}
