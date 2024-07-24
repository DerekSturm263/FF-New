using Codice.CM.Common;
using Extensions.Miscellaneous;
using NUnit.Framework;
using Quantum;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationEventContainerWindow : EditorWindow
{
    public static AnimationEventContainerWindow Instance;

    public static FrameEventAsset Selected;

    public int _scrubFrame;
    public int _maxScrubFrame = 60;

    private QuantumAnimationEventAsset _eventAsset;
    private AnimationClip _animation;
    private GameObject _animationPreview;

    private float _initialTime;
    private bool _isPreviewing;

    [MenuItem("Fusion Fighters/Animation Event Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationEventContainerWindow), false, "Animation Event Editor");
    }

    private void OnGUI()
    {
        Instance = this;

        GUILayout.BeginHorizontal();

        _eventAsset = EditorGUILayout.ObjectField(_eventAsset, typeof(QuantumAnimationEventAsset), false) as QuantumAnimationEventAsset;
        _animation = EditorGUILayout.ObjectField(_animation, typeof(AnimationClip), false) as AnimationClip;
        _animationPreview = EditorGUILayout.ObjectField(_animationPreview, typeof(GameObject), true) as GameObject;

        _maxScrubFrame = (int)(_animation.length * 60);

        GUILayout.EndHorizontal();

        if (!_eventAsset || !_animation)
        {
            EditorGUILayout.HelpBox("Please assign both an Animation Event Asset and an Animation Clip to begin editting and previewing events. The animation you're previewing should be the one that the Event will be used with", MessageType.Info);
            return;
        }

        if (!_animationPreview)
        {
            EditorGUILayout.HelpBox("Please assign a GameObject to preview the events with", MessageType.Warning);
        }

        Rect rect1 = EditorGUILayout.GetControlRect();
        rect1.width = rect1.height;

        if (GUI.Button(rect1, new GUIContent(">", "Previews the Animation Event with the given Animation Clip")))
        {
            _isPreviewing = !_isPreviewing;
            _initialTime += Time.time;
        }

        Rect rect2 = rect1;
        rect2.x += rect1.width;

        if (GUI.Button(rect2, new GUIContent("+", "Adds a new Event starting at the given frame")))
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            PopupWindow.Show(lastRect, new FrameEventPopup(_scrubFrame, _eventAsset, _eventAsset));
        }

        Rect rect3 = rect2;
        rect3.x += rect2.width;

        if (GUI.Button(rect3, new GUIContent("-", "Removes the currently selected event")))
        {
            AssetDatabase.RemoveObjectFromAsset(Selected);

            AssetRefFrameEvent toRemove = _eventAsset.Settings.Events.Find(item => item.Id == Selected.AssetObject.Guid);
            _eventAsset.Settings.Events.Remove(toRemove);

            AssetDatabase.SaveAssets();

            Selected = null;
        }

        if (_isPreviewing)
        {
            _scrubFrame = (int)((Time.time - _initialTime) / 60) % _animation.FrameCount();
        }

        Rect sliderRect = EditorGUILayout.GetControlRect();
        sliderRect.width -= rect1.width * 4 + 10;
        sliderRect.x += rect1.width * 4 + 10;
        sliderRect.y -= EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginDisabledGroup(_isPreviewing);

        _scrubFrame = EditorGUI.IntSlider(sliderRect, _scrubFrame, 0, _maxScrubFrame);

        EditorGUI.EndDisabledGroup();

        for (int i = 0; i < _eventAsset.Settings.Events.Count; ++i)
        {
            FrameEventAsset frameEvent = UnityDB.FindAssetForInspector(_eventAsset.Settings.Events[i].Id) as FrameEventAsset;
            if (!frameEvent)
                continue;

            float startingFrame = 0;
            float endingFrame = 0;

            if (frameEvent is SpawnHitboxEventAsset hitbox)
            {
                startingFrame = hitbox.Settings.StartingFrame;
                endingFrame = hitbox.Settings.EndingFrame;
            }
            else if (frameEvent is SpawnProjectileEventAsset projectile)
            {
                startingFrame = projectile.Settings.StartingFrame;
                endingFrame = projectile.Settings.EndingFrame;
            }
            else if (frameEvent is ModifyHurtboxesEventAsset hurtboxes)
            {
                startingFrame = hurtboxes.Settings.StartingFrame;
                endingFrame = hurtboxes.Settings.EndingFrame;
            }
            else if (frameEvent is SpawnVFXEventAsset vfx)
            {
                startingFrame = vfx.Settings.StartingFrame;
                endingFrame = vfx.Settings.EndingFrame;
            }
            else if (frameEvent is PlayClipEventAsset clip)
            {
                startingFrame = clip.Settings.StartingFrame;
                endingFrame = clip.Settings.EndingFrame;
            }
            else if (frameEvent is ApplyPhysicsEventAsset physics)
            {
                startingFrame = physics.Settings.StartingFrame;
                endingFrame = physics.Settings.EndingFrame;
            }

            if (frameEvent != Selected)
            {
                if (frameEvent is SpawnHitboxEventAsset)
                    GUI.color = Color.red;
                else if (frameEvent is SpawnProjectileEventAsset)
                    GUI.color = Color.cyan;
                else if (frameEvent is ModifyHurtboxesEventAsset)
                    GUI.color = Color.blue;
                else if (frameEvent is SpawnVFXEventAsset)
                    GUI.color = Color.green;
                else if (frameEvent is PlayClipEventAsset)
                    GUI.color = Color.magenta;
                else if (frameEvent is ApplyPhysicsEventAsset)
                    GUI.color = Color.yellow;
            }

            EditorGUILayout.MinMaxSlider($"[{startingFrame} - {endingFrame}]: {frameEvent.name}", ref startingFrame, ref endingFrame, 0, _maxScrubFrame);
            GUI.color = Color.white;

            if (frameEvent is SpawnHitboxEventAsset hitbox2)
            {
                hitbox2.Settings.StartingFrame = (int)startingFrame;
                hitbox2.Settings.EndingFrame = (int)endingFrame;
            }
            else if (frameEvent is SpawnProjectileEventAsset projectile)
            {
                projectile.Settings.StartingFrame = (int)startingFrame;
                projectile.Settings.EndingFrame = (int)endingFrame;
            }
            else if (frameEvent is ModifyHurtboxesEventAsset hurtboxes)
            {
                hurtboxes.Settings.StartingFrame = (int)startingFrame;
                hurtboxes.Settings.EndingFrame = (int)endingFrame;
            }
            else if (frameEvent is SpawnVFXEventAsset vfx)
            {
                vfx.Settings.StartingFrame = (int)startingFrame;
                vfx.Settings.EndingFrame = (int)endingFrame;
            }
            else if (frameEvent is PlayClipEventAsset clip)
            {
                clip.Settings.StartingFrame = (int)startingFrame;
                clip.Settings.EndingFrame = (int)endingFrame;
            }
            else if (frameEvent is ApplyPhysicsEventAsset physics)
            {
                physics.Settings.StartingFrame = (int)startingFrame;
                physics.Settings.EndingFrame = (int)endingFrame;
            }

            if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                Selected = frameEvent;
                Selection.activeObject = Selected;
            }
        }

        PreviewAnimation();
    }

    public void PreviewAnimation()
    {
        if (!_animationPreview)
            _animationPreview = GameObject.FindGameObjectWithTag("Player");

        if (_animation && _animationPreview)
            _animation.SampleAnimation(_animationPreview, (float)_scrubFrame / _maxScrubFrame);
    }

    public unsafe void DrawGizmos()
    {
        if (!_eventAsset || !_animationPreview)
            return;

        for (int i = 0; i < _eventAsset.Settings.Events.Count; ++i)
        {
            FrameEventAsset frameEvent = UnityDB.FindAssetForInspector(_eventAsset.Settings.Events[i].Id) as FrameEventAsset;
            if (!frameEvent)
                continue;

            int startingFrame = 0;
            int endingFrame = 0;

            if (frameEvent is SpawnHitboxEventAsset hitbox)
            {
                startingFrame = hitbox.Settings.StartingFrame;
                endingFrame = hitbox.Settings.EndingFrame;
            }
            else if (frameEvent is SpawnProjectileEventAsset projectile)
            {
                startingFrame = projectile.Settings.StartingFrame;
                endingFrame = projectile.Settings.EndingFrame;
            }
            else if (frameEvent is ModifyHurtboxesEventAsset hurtboxes)
            {
                startingFrame = hurtboxes.Settings.StartingFrame;
                endingFrame = hurtboxes.Settings.EndingFrame;
            }
            else if (frameEvent is SpawnVFXEventAsset vfx)
            {
                startingFrame = vfx.Settings.StartingFrame;
                endingFrame = vfx.Settings.EndingFrame;
            }
            else if (frameEvent is PlayClipEventAsset clip)
            {
                startingFrame = clip.Settings.StartingFrame;
                endingFrame = clip.Settings.EndingFrame;
            }
            else if (frameEvent is ApplyPhysicsEventAsset physics)
            {
                startingFrame = physics.Settings.StartingFrame;
                endingFrame = physics.Settings.EndingFrame;
            }

            if (_scrubFrame >= startingFrame && _scrubFrame <= endingFrame)
            {
                if (frameEvent is SpawnHitboxEventAsset spawnHitbox)
                {
                    Gizmos.color = Color.Lerp(Color.white, Color.red, (float)spawnHitbox.Settings.Settings.Damage / 100);

                    for (int j = 0; j < spawnHitbox.Settings.Shape.CompoundShapes.Length; ++j)
                    {
                        switch (spawnHitbox.Settings.Shape.CompoundShapes[j].ShapeType)
                        {
                            case Shape2DType.Circle:
                                Gizmos.DrawSphere(spawnHitbox.Settings.Shape.CompoundShapes[j].PositionOffset.ToUnityVector3(), spawnHitbox.Settings.Shape.CompoundShapes[j].CircleRadius.AsFloat);
                                break;

                            case Shape2DType.Box:
                                Gizmos.DrawCube(spawnHitbox.Settings.Shape.CompoundShapes[j].PositionOffset.ToUnityVector3(), spawnHitbox.Settings.Shape.CompoundShapes[j].BoxExtents.ToUnityVector3());
                                break;
                        }
                    }

                    Gizmos.DrawLineList(CalculateArcPositions(20, spawnHitbox.Settings.Settings.Knockback.ToUnityVector2(), spawnHitbox.Settings.Shape.CompoundShapes[^1].PositionOffset.ToUnityVector2()));
                }
                else if (frameEvent is ModifyHurtboxesEventAsset modifyHurtboxes)
                {

                }
                else if (frameEvent is ApplyPhysicsEventAsset applyPhysics)
                {

                }
                else if (frameEvent is SpawnProjectileEventAsset spawnProjectile)
                {

                }
            }
        }
    }

    private ReadOnlySpan<Vector3> CalculateArcPositions(int resolution, Vector2 amount, Vector2 offset)
    {
        Vector3[] positions = new Vector3[resolution];

        for (int i = 0; i < resolution; ++i)
        {
            float t = (float)i / resolution;
            positions[i] = CalculateArcPoint(t, 20, 1, amount) + offset;
        }

        return positions;
    }

    private Vector2 CalculateArcPoint(float t, float gravity, float scalar, Vector2 amount)
    {
        amount.x += 0.0001f;
        float angle = Mathf.Atan2(amount.y, amount.x);

        float x = t * amount.x;
        float y = x * Mathf.Tan(angle) - (gravity * x * x / (2 * amount.magnitude * amount.magnitude * Mathf.Cos(angle) * Mathf.Cos(angle)));

        return new Vector2(x, y) * scalar;
    }
}