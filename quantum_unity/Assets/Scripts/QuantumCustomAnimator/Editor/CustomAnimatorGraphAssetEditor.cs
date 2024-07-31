using Photon.Deterministic;
using UnityEditor;
using UnityEngine;
using Quantum.Custom.Animator;

[CustomEditor(typeof(CustomAnimatorGraphAsset))]
public class CustomAnimatorGraphAssetEditor : Editor
{
    private CustomAnimatorGraphAsset asset = null;
    private int selectedLayer = 0;
    private int selectedState = 0;
    private int selectedBlend = 0;
    private bool showMovementData;

    private void OnEnable()
    {
        asset = (CustomAnimatorGraphAsset)target;
    }

    public override void OnInspectorGUI()
    {
        var graph = asset.Settings;

        CustomEditorsHelper.Header("CustomAnimatorGraphAsset");

        if (GUILayout.Button("Import Mecanim Controller"))
        {
            CustomExportAnimations.CreateAsset(asset, (UnityEditor.Animations.AnimatorController)asset.controller);

            EditorUtility.SetDirty(asset);
            AssetDatabase.Refresh();
        }
        /*
        if (graph != null && graph.layers != null)
        {
          Quantum.CustomAnimatorGraph.Deserialize(graph);

          int layerCount = graph.layers.Length;
          if (layerCount > 0)
          {
            string[] layerNames = new string[layerCount];
            for (int l = 0; l < layerCount; l++)
              layerNames[l] = graph.layers[l].name;
            selectedLayer = GUILayout.Toolbar(selectedLayer, layerNames);
            AnimatorLayer layer = graph.layers[selectedLayer];

            if (layer != null)
            {
              int stateCount = layer.states.Length;

              if (stateCount > 0)
              {
                string[] stateNames = new string[stateCount];
                for (int s = 0; s < stateCount; s++)
                  stateNames[s] = layer.states[s].name;
                selectedState = GUILayout.Toolbar(selectedState, stateNames);
                AnimatorState state = layer.states[selectedState];

                if (state != null)
                {
                  EditorGUILayout.BeginVertical("box");

                  EditorGUILayout.LabelField("State");
                  EditorGUILayout.LabelField(state.name);
                  EditorGUILayout.LabelField(string.Format("Is Default: {0}", state.isDefault));
                  if (state.motion != null)
                  {
                    EditorGUILayout.LabelField(string.Format("Is Blend Tree: {0}", state.motion.isTree));

                    if (!state.motion.isTree)
                    {
                      AnimatorClip motion = state.motion as AnimatorClip;
                      AnimationClipGui(motion);
                    }
                    else
                    {
                      AnimatorBlendTree motion = state.motion as AnimatorBlendTree;
                      string[] blends = new string[motion.motionCount];
                      for (int b = 0; b < motion.motionCount; b++)
                        blends[b] = motion.motions[b].name;
                      selectedBlend = SelectionField(blends, selectedBlend, 2);

                      AnimatorClip clip = motion.motions[selectedBlend] as AnimatorClip;
                      AnimationClipGui(clip);
                    }

                  }
                  else
                  {
                    EditorGUILayout.LabelField("No Motion Set");
                                }

                  EditorGUILayout.LabelField(string.Format("Events: {0}", state.StateAsset.Id));


                                EditorGUILayout.Space();
                  EditorGUILayout.LabelField("Transitions");
                  int transitionCount = state.transitions.Length;
                  for (int t = 0; t < transitionCount; t++)
                  {
                    EditorGUILayout.BeginVertical("Box");

                    AnimatorTransition transition = state.transitions[t];

                    EditorGUILayout.LabelField(string.Format("{0}. {1}", transition.index, transition.name));
                    EditorGUILayout.LabelField(string.Format("Duration: {0} sec", transition.duration.AsFloat));
                    EditorGUILayout.LabelField(string.Format("Has Exit Time: {0}", transition.hasExitTime));
                    EditorGUILayout.LabelField(string.Format("Exit Time: {0} sec", transition.exitTime.AsFloat));
                    EditorGUILayout.LabelField(string.Format("Destination State (Hash): {0} ({1})", transition.destinationStateName, transition.destinationStateId));
                    EditorGUILayout.LabelField(string.Format("Offset: {0} sec", transition.offset.AsFloat));

                    int conditionCount = transition.conditions.Length;
                    for (int c = 0; c < conditionCount; c++)
                    {
                      EditorGUILayout.BeginVertical("Box");
                      AnimatorCondition condition = transition.conditions[c];
                      AnimatorVariable variable = graph.variables[graph.VariableIndex(condition.variableName)];

                      string conditionMode = "";
                      switch (condition.mode)
                      {
                        case AnimatorCondition.Modes.Equals:
                          conditionMode = "==";
                          break;
                        case AnimatorCondition.Modes.Greater:
                          conditionMode = ">";
                          break;
                        case AnimatorCondition.Modes.If:
                          conditionMode = "is true";
                          break;
                        case AnimatorCondition.Modes.IfNot:
                          conditionMode = "is false";
                          break;
                        case AnimatorCondition.Modes.Less:
                          conditionMode = "<";
                          break;
                        case AnimatorCondition.Modes.NotEqual:
                          conditionMode = "!=";
                          break;
                      }

                      string threshold = "";
                      switch (variable.type)
                      {
                        case AnimatorVariable.VariableType.FP:
                          threshold = condition.thresholdFp.AsFloat.ToString();
                          break;
                        case AnimatorVariable.VariableType.Int:
                          threshold = condition.thresholdInt.ToString();
                          break;
                      }

                      EditorGUILayout.LabelField(string.Format("\"{0}\" - {1} - {2}", condition.variableName, conditionMode, threshold));

                      EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                  }

                  EditorGUILayout.EndVertical();
                }
              }
            }
          }
        }


        base.OnInspectorGUI();
            */
    }

    private void AnimationClipGui(AnimatorClip clip)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(EditorGUIUtility.currentViewWidth - 20));
        EditorGUILayout.LabelField("Animator Clip");
        EditorGUILayout.LabelField(string.Format("Name: {0}", clip.clipName));
        EditorGUILayout.LabelField(string.Format("Length: {0}", clip.data.length.AsFloat));
        EditorGUILayout.LabelField(string.Format("Frame Rate: {0}", clip.data.frameRate));
        EditorGUILayout.LabelField(string.Format("Frame Count: {0}", clip.data.frameCount));
        EditorGUILayout.LabelField(string.Format("Looped: {0}", clip.data.looped));
        EditorGUILayout.LabelField(string.Format("Mirrored: {0}", clip.data.mirror));

        int frameCount = clip.data.frameCount;
        Vector3[] positions = new Vector3[frameCount];
        Quaternion[] rotationsQ = new Quaternion[frameCount];
        Vector3[] rotations = new Vector3[frameCount];
        float[] times = new float[frameCount];
        HurtboxTransformInfo[][] hurtboxPositions = new HurtboxTransformInfo[frameCount][];
        for (int f = 0; f < frameCount; f++)
        {
            AnimatorFrame frame = clip.data.frames[f];
            float frameTime = frame.time.AsFloat;
            FPVector3 position = frame.position;
            FPQuaternion rotation = frame.rotation;

            HurtboxTransformInfo[] hurtboxPositions2 = frame.hurtboxPositions;

            Vector3 pV3 = new Vector3(position.X.AsFloat, position.Y.AsFloat, position.Z.AsFloat);
            Quaternion rQ = new Quaternion(rotation.X.AsFloat, rotation.Y.AsFloat, rotation.Z.AsFloat, rotation.W.AsFloat);
            Vector3 rV3 = rQ.eulerAngles;

            positions[f] = pV3;
            rotationsQ[f] = rQ;
            rotations[f] = rV3;
            times[f] = frameTime;
            hurtboxPositions[f] = hurtboxPositions2;
        }

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(string.Format("Delta Movement: {0}", (positions[frameCount - 1] - positions[0]).ToString("F3")));
        Quaternion deltaQ = Quaternion.FromToRotation(rotationsQ[0] * Vector3.forward, rotationsQ[frameCount - 1] * Vector3.forward);
        Vector3 deltaQV = deltaQ.eulerAngles;
        if (deltaQV.x > 180) deltaQV.x += -360;
        if (deltaQV.y > 180) deltaQV.y += -360;
        if (deltaQV.z > 180) deltaQV.z += -360;
        EditorGUILayout.LabelField(string.Format("Delta Rotation: {0}", deltaQV.ToString("F3")));

        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical("box");
        if (showMovementData = EditorGUILayout.Foldout(showMovementData, "Movement Data"))
        {
            EditorGUILayout.BeginHorizontal();


            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Times", GUILayout.Width(75));
            for (int f = 0; f < frameCount; f++)
                EditorGUILayout.LabelField(times[f].ToString("F3"), GUILayout.Width(75));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Positions", GUILayout.Width(160));
            for (int f = 0; f < frameCount; f++)
                EditorGUILayout.LabelField(positions[f].ToString("F2"), GUILayout.Width(160));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Rotations", GUILayout.Width(160));
            for (int f = 0; f < frameCount; f++)
                EditorGUILayout.LabelField(rotations[f].ToString("F2"), GUILayout.Width(160));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private int SelectionField(string[] options, int selection, int xCount)
    {
        int libraryCount = options.Length;
        int yCount = Mathf.CeilToInt(libraryCount / (float)xCount);

        float calSize = (EditorGUIUtility.currentViewWidth) / xCount;

        EditorGUILayout.BeginVertical();
        for (int y = 0; y < yCount; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < xCount; x++)
            {
                int index = x + y * xCount;
                if (index < libraryCount)
                {
                    if (index != selection)
                    {
                        if (GUILayout.Button(options[index], GUILayout.Width(calSize)))
                            return index;
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal("box", GUILayout.Width(calSize));
                        EditorGUILayout.LabelField(options[index], GUILayout.Width(calSize));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
        EditorGUILayout.EndVertical();

        return selection;
    }
}
