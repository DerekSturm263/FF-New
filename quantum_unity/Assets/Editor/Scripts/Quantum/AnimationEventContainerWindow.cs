using Extensions.Miscellaneous;
using Photon.Deterministic;
using Quantum;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class AnimationEventContainerWindow : EditorWindow
{
    public static AnimationEventContainerWindow Instance;

    public static FrameEventAsset Selected;

    public int _scrubFrame;
    public int _maxScrubFrame;

    private QuantumAnimationEventAsset _eventAsset;

    private GameObject _previewPlayer;
    public GameObject PreviewPlayer => _previewPlayer;

    private HurtboxTracker _previewTracker;

    private GameObject _previewWeapon;
    public GameObject PreviewWeapon => _previewWeapon;

    private float _initialTime;
    private bool _isPreviewing;

    private ReorderableList _events;

    [MenuItem("Fusion Fighters/Animation Event Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationEventContainerWindow), false, "Animation Event Editor");
    }

    private void OnEnable()
    {
        EditorApplication.update += Preview;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Preview;
    }

    public void EventListDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        FrameEventAsset frameEvent = UnityDB.FindAssetForInspector(_eventAsset.Settings.Events[index].Id) as FrameEventAsset;

        float startingFrame = 0;
        float endingFrame = 0;

        if (frameEvent is SpawnHitboxEventAsset hitbox)
        {
            startingFrame = hitbox.Settings.StartingFrame;
            endingFrame = hitbox.Settings.EndingFrame;
        }
        else if (frameEvent is SpawnItemEventAsset item)
        {
            startingFrame = item.Settings.StartingFrame;
            endingFrame = item.Settings.EndingFrame;
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
        else if (frameEvent is HoldAnimationEventAsset hold)
        {
            startingFrame = hold.Settings.StartingFrame;
            endingFrame = hold.Settings.EndingFrame;
        }
        else if (frameEvent is ContinueAnimationEventAsset continueAnim)
        {
            startingFrame = continueAnim.Settings.StartingFrame;
            endingFrame = continueAnim.Settings.EndingFrame;
        }
        else if (frameEvent is ExecuteUnityEventEventAsset unityEvent)
        {
            startingFrame = unityEvent.Settings.StartingFrame;
            endingFrame = unityEvent.Settings.EndingFrame;
        }
        else if (frameEvent is AllowMovementEventAsset movement)
        {
            startingFrame = movement.Settings.StartingFrame;
            endingFrame = movement.Settings.EndingFrame;
        }

        Rect nameRect = new(rect.x, rect.y, 149, rect.height);

        string newName = EditorGUI.TextField(nameRect, frameEvent.name);
        if (newName != frameEvent.name)
        {
            var path = AssetDatabase.GUIDToAssetPath(AssetDatabase.GetAssetPath(frameEvent));
            AssetDatabase.RenameAsset(path, newName);
        }

        GUI.color = frameEvent.Color;

        Rect sliderRect = new(nameRect.x + nameRect.width + 4, nameRect.y, rect.width - 204, nameRect.height);
        EditorGUI.MinMaxSlider(sliderRect, ref startingFrame, ref endingFrame, 0, _maxScrubFrame);

        GUI.color = Color.white;

        Rect minRect = new(sliderRect.x + sliderRect.width + 5, sliderRect.y, 20, sliderRect.height);
        startingFrame = EditorGUI.IntField(minRect, (int)startingFrame);

        Rect maxRect = new(minRect.x + minRect.width + 8, minRect.y, minRect.width, minRect.height);
        endingFrame = EditorGUI.IntField(maxRect, (int)endingFrame);

        if (frameEvent is SpawnHitboxEventAsset hitbox2)
        {
            hitbox2.Settings.StartingFrame = (int)startingFrame;
            hitbox2.Settings.EndingFrame = (int)endingFrame;
        }
        else if (frameEvent is SpawnItemEventAsset item)
        {
            item.Settings.StartingFrame = (int)startingFrame;
            item.Settings.EndingFrame = (int)endingFrame;
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
        else if (frameEvent is HoldAnimationEventAsset hold)
        {
            hold.Settings.StartingFrame = (int)startingFrame;
            hold.Settings.EndingFrame = (int)endingFrame;
        }
        else if (frameEvent is ContinueAnimationEventAsset continueAnim)
        {
            continueAnim.Settings.StartingFrame = (int)startingFrame;
            continueAnim.Settings.EndingFrame = (int)endingFrame;
        }
        else if (frameEvent is ExecuteUnityEventEventAsset unityEvent)
        {
            unityEvent.Settings.StartingFrame = (int)startingFrame;
            unityEvent.Settings.EndingFrame = (int)endingFrame;
        }
        else if (frameEvent is AllowMovementEventAsset movement)
        {
            movement.Settings.StartingFrame = (int)startingFrame;
            movement.Settings.EndingFrame = (int)endingFrame;
        }

        if (isFocused)
        {
            Selected = frameEvent;
            Selection.activeObject = Selected;
        }
    }

    public float EventListElementHeightCallback(int index) => EditorGUIUtility.singleLineHeight;

    public void EventListOnAddCallback(ReorderableList list)
    {
        Rect rect = new(Event.current.mousePosition, new(500, 700));
        PopupWindow.Show(rect, new FrameEventPopup(_scrubFrame, _scrubFrame + 1, _eventAsset));
    }

    public void EventListOnRemoveCallback(ReorderableList list)
    {
        AssetDatabase.RemoveObjectFromAsset(Selected);

        AssetRefFrameEvent toRemove = _eventAsset.Settings.Events.Find(item => item.Id == Selected.AssetObject.Guid);
        _eventAsset.Settings.Events.Remove(toRemove);

        AssetDatabase.SaveAssets();

        Selected = null;
    }

    private void OnGUI()
    {
        Instance = this;

        GUILayout.BeginHorizontal();

        QuantumAnimationEventAsset eventAsset = EditorGUILayout.ObjectField("Animation Event", _eventAsset, typeof(QuantumAnimationEventAsset), false) as QuantumAnimationEventAsset;
        if (eventAsset != _eventAsset)
        {
            _eventAsset = eventAsset;

            _events = new(_eventAsset.Settings.Events, typeof(AssetRefFrameEvent), true, false, true, true)
            {
                drawElementCallback = EventListDrawElementCallback,
                elementHeightCallback = EventListElementHeightCallback,
                onAddCallback = EventListOnAddCallback,
                onRemoveCallback = EventListOnRemoveCallback,
            };
        }

        GUILayout.Space(40);

        _previewPlayer = EditorGUILayout.ObjectField("Player", _previewPlayer, typeof(GameObject), true) as GameObject;
        _previewWeapon = EditorGUILayout.ObjectField("Weapon", _previewWeapon, typeof(GameObject), true) as GameObject;

        GUILayout.EndHorizontal();

        if (!_eventAsset)
        {
            EditorGUILayout.HelpBox("Please assign an AnimationEvent for editing", MessageType.Error);
            return;
        }

        _events ??= new(_eventAsset.Settings.Events, typeof(AssetRefFrameEvent), true, false, true, true)
        {
            drawElementCallback = EventListDrawElementCallback,
            elementHeightCallback = EventListElementHeightCallback,
            onAddCallback = EventListOnAddCallback,
            onRemoveCallback = EventListOnRemoveCallback
        };

        if (!_previewPlayer || !_previewWeapon)
        {
            EditorGUILayout.HelpBox("Please assign a\n- Player GameObject for previewing\n- Weapon GameObject for previewing", MessageType.Warning);
        }

        GUILayout.Space(10);

        _maxScrubFrame = _eventAsset.Clip.FrameCount();

        float minCommittedFrame = _eventAsset.Settings.Committed.Min;
        float maxCommittedFrame = _eventAsset.Settings.Committed.Max;
        EditorGUILayout.MinMaxSlider(new GUIContent("Committed Frames", "While the player is between these frames, they can not cancel the attack"), ref minCommittedFrame, ref maxCommittedFrame, 0, _maxScrubFrame);
        _eventAsset.Settings.Committed.Min = (int)minCommittedFrame;
        _eventAsset.Settings.Committed.Max = (int)maxCommittedFrame;

        GUILayout.BeginHorizontal();

        if (!_isPreviewing)
        {
            if (GUILayout.Button(new GUIContent("Start Preview", "Previews the Animation Event by playing it"), GUILayout.MaxWidth(168)))
            {
                _isPreviewing = true;
                _initialTime = Time.realtimeSinceStartup;
            }
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Stop Preview", "Stops previewing the Animation Event"), GUILayout.MaxWidth(168)))
            {
                _isPreviewing = false;
            }
        }

        EditorGUI.BeginDisabledGroup(_isPreviewing);

        if (_isPreviewing)
        {
            EditorGUILayout.IntSlider(_scrubFrame, 0, _maxScrubFrame);
        }
        else
        {
            _scrubFrame = EditorGUILayout.IntSlider(_scrubFrame, 0, _maxScrubFrame);
        }

        GUILayout.EndHorizontal();

        _events.DoLayoutList();

        EditorGUI.EndDisabledGroup();
    }

    private void Preview()
    {
        if (_isPreviewing)
        {
            float elapsedTime = Time.realtimeSinceStartup - _initialTime;
            float elapsedFrames = elapsedTime * _eventAsset.Clip.frameRate;

            _scrubFrame = (int)elapsedFrames % _maxScrubFrame;
        }

        SampleAnimation();
    }

    public void SampleAnimation()
    {
        if (!_previewPlayer)
            _previewPlayer = GameObject.FindGameObjectWithTag("Player");

        if (!_previewWeapon)
            _previewWeapon = GameObject.Find("HANDLE_GRIP");
        
        if (_eventAsset && _previewPlayer)
            _eventAsset.Clip.SampleAnimation(_previewPlayer, _scrubFrame / _eventAsset.Clip.frameRate);
    }

    public unsafe void DrawGizmos(AssetRefHurtboxSetup hurtboxSetup)
    {
        if (!_eventAsset || !_previewPlayer || !_previewWeapon)
            return;

        Color[] hurtboxColors = new Color[] { Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white, Color.white };

        for (int i = 0; i < _eventAsset.Settings.Events.Count; ++i)
        {
            FrameEventAsset frameEvent = UnityDB.FindAssetForInspector(_eventAsset.Settings.Events[i].Id) as FrameEventAsset;

            int startingFrame = 0;
            int endingFrame = 0;

            if (frameEvent is SpawnHitboxEventAsset hitbox)
            {
                startingFrame = hitbox.Settings.StartingFrame;
                endingFrame = hitbox.Settings.EndingFrame;
            }
            else if (frameEvent is SpawnItemEventAsset item)
            {
                startingFrame = item.Settings.StartingFrame;
                endingFrame = item.Settings.EndingFrame;
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
            else if (frameEvent is HoldAnimationEventAsset hold)
            {
                startingFrame = hold.Settings.StartingFrame;
                endingFrame = hold.Settings.EndingFrame;
            }
            else if (frameEvent is ContinueAnimationEventAsset continueAnim)
            {
                startingFrame = continueAnim.Settings.StartingFrame;
                endingFrame = continueAnim.Settings.EndingFrame;
            }
            else if (frameEvent is ExecuteUnityEventEventAsset unityEvent)
            {
                startingFrame = unityEvent.Settings.StartingFrame;
                endingFrame = unityEvent.Settings.EndingFrame;
            }
            else if (frameEvent is AllowMovementEventAsset movement)
            {
                startingFrame = movement.Settings.StartingFrame;
                endingFrame = movement.Settings.EndingFrame;
            }

            if (_scrubFrame >= startingFrame && _scrubFrame <= endingFrame)
            {
                Gizmos.color = frameEvent.Color;

                if (frameEvent is SpawnHitboxEventAsset spawnHitbox)
                    PreviewHitbox(frameEvent.name, spawnHitbox.Settings, Gizmos.color);
                else if (frameEvent is ModifyHurtboxesEventAsset modifyHurtboxes)
                    PreviewHurtboxes(modifyHurtboxes.Settings, ref hurtboxColors);
                else if (frameEvent is ApplyPhysicsEventAsset applyPhysics)
                    PreviewPhysics(applyPhysics.Settings);
            }
        }

        _previewTracker ??= _previewPlayer.GetComponent<HurtboxTracker>();

        for (int i = 0; i < 15; ++i)
        {
            Transform hurtboxTransform = _previewTracker.GetHurtbox((Quantum.HurtboxType)(1 << i));
            HurtboxSetupAsset asset = UnityDB.FindAsset<HurtboxSetupAsset>(hurtboxSetup.Id);

            Gizmos.color = hurtboxColors[i];
            Gizmos.DrawSphere(hurtboxTransform.position, asset.Settings.HurtboxSizes[i].AsFloat);
        }
    }

    private void PreviewHitbox(string label, SpawnHitboxEvent eventSettings, Color color)
    {
        Vector3 offset;

        if (eventSettings.Parent == SpawnHitboxEvent.ParentType.Player)
            offset = _previewPlayer.transform.position;
        else
            offset = _previewWeapon.transform.position;

        for (int i = 0; i < eventSettings.Shape.CompoundShapes.Length; ++i)
        {
            Vector3 position = offset;

            if (eventSettings.Parent == SpawnHitboxEvent.ParentType.Weapon)
            {
                position += _previewWeapon.transform.up * eventSettings.Shape.CompoundShapes[i].PositionOffset.Y.AsFloat + _previewWeapon.transform.right * eventSettings.Shape.CompoundShapes[i].PositionOffset.X.AsFloat;
            }
            else
            {
                position += new Vector3(eventSettings.Shape.CompoundShapes[i].PositionOffset.X.AsFloat, eventSettings.Shape.CompoundShapes[i].PositionOffset.Y.AsFloat);
            }

            switch (eventSettings.Shape.CompoundShapes[i].ShapeType)
            {
                case Shape2DType.Circle:
                    Gizmos.DrawSphere(position, eventSettings.Shape.CompoundShapes[i].CircleRadius.AsFloat);
                    break;

                case Shape2DType.Box:
                    Gizmos.DrawCube(position, eventSettings.Shape.CompoundShapes[i].BoxExtents.ToUnityVector3());
                    break;
            }

            if (i == eventSettings.Shape.CompoundShapes.Length / 2)
                PreviewKnockback(eventSettings, position);
        }

        Handles.Label(offset + new Vector3(0.75f, 0.75f), label);
    }

    private void PreviewKnockback(SpawnHitboxEvent eventSettings, Vector2 offset)
    {
        Gizmos.DrawLineList(CalculateArcPositions(20, eventSettings.UnchargedSettings.Offensive.Knockback.ToUnityVector2(), offset));

        if (eventSettings.FullyChargedSettings.Offensive.Knockback != FPVector2.Zero)
            Gizmos.DrawLineList(CalculateArcPositions(20, eventSettings.FullyChargedSettings.Offensive.Knockback.ToUnityVector2(), offset));
    }

    private void PreviewHurtboxes(ModifyHurtboxesEvent eventSettings, ref Color[] colors)
    {
        for (int i = 0; i < colors.Length; ++i)
        {
            HurtboxType hurtboxType = (HurtboxType)Math.Pow(2, i);

            if (eventSettings.Hurtboxes.HasFlag(hurtboxType))
            {
                colors[i].r = eventSettings.Settings.CanBeDamaged ? 1 : 0;
                colors[i].g = eventSettings.Settings.CanBeKnockedBack ? 1 : 0;
                colors[i].b = eventSettings.Settings.CanBeInterrupted ? 1 : 0;
            }
        }
    }

    private void PreviewPhysics(ApplyPhysicsEvent eventSettings)
    {
        int elapsedFrames = _scrubFrame - eventSettings.StartingFrame;
        FP normalizedTime = (FP)elapsedFrames / eventSettings.Length;

        _previewPlayer.transform.parent.position = ApplyPhysicsEvent.GetPositionAtTime(eventSettings.UnchargedSettings, normalizedTime, 1).ToUnityVector2();
    }

    private static ReadOnlySpan<Vector3> CalculateArcPositions(int resolution, Vector2 amount, Vector2 offset)
    {
        Vector3[] positions = new Vector3[resolution];

        for (int i = 0; i < resolution; ++i)
        {
            float t = (float)i / resolution;
            positions[i] = CalculateArcPoint(t, 20, 1, amount) + offset;
        }

        return positions;
    }

    private static Vector2 CalculateArcPoint(float t, float gravity, float scalar, Vector2 amount)
    {
        amount.x += 0.0001f;
        float angle = Mathf.Atan2(amount.y, amount.x);

        float x = t * amount.x;
        float y = x * Mathf.Tan(angle) - (gravity * x * x / (2 * amount.magnitude * amount.magnitude * Mathf.Cos(angle) * Mathf.Cos(angle)));

        return new Vector2(x, y) * scalar;
    }
}