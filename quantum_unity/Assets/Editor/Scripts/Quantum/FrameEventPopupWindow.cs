using System.Linq;
using UnityEditor;
using UnityEngine;

internal class FrameEventPopup : PopupWindowContent
{
    private int _beginningFrame;
    private int _endingFrame;
    private QuantumAnimationEventAsset _events;
    private Object _parent;

    private string _name;
    private System.Type _type;
    private bool[] _clicked;

    public FrameEventPopup(int beginningFrame, int endingFrame, QuantumAnimationEventAsset events)
    {
        _beginningFrame = beginningFrame;
        _endingFrame = endingFrame;
        _events = events;
        _parent = events;
        _clicked = new bool[] { false, false, false, false, false, false, false, false, false, false };

        _name = $"Event {events.Settings.Events.Count + 1}";
    }

    public override void OnGUI(Rect rect)
    {
        _name = EditorGUILayout.TextField(_name);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(_clicked[0]);
        if (GUILayout.Button(new GUIContent("Weapon", "Sets a weapon's static hitbox as active for the duration of the frames")))
        {
            _type = typeof(SetWeaponHitboxEventAsset);
            _clicked = new bool[] { true, false, false, false, false, false, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[9]);
        if (GUILayout.Button(new GUIContent("Hitbox", "Spawns a hitbox during the active frames. When a hitbox hits another player's hurtbox, the attack is resolved")))
        {
            _type = typeof(SpawnDynamicHitboxEventAsset);
            _clicked = new bool[] { false, false, false, false, false, false, false, false, false, true };
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(_clicked[3]);
        if (GUILayout.Button(new GUIContent("Hurtbox", "Modifies the state of the player's hurtboxes. This can be used to create super armor, heavy armor, etc.")))
        {
            _type = typeof(ModifyHurtboxesEventAsset);
            _clicked = new bool[] { false, false, false, true, false, false, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[8]);
        if (GUILayout.Button(new GUIContent("Movement", "Allows the player to enter certain movement states. This can be used to let the player move, jump, etc. at any point during the attack")))
        {
            _type = typeof(AllowMovementEventAsset);
            _clicked = new bool[] { false, false, false, false, false, false, false, false, true, false };
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(_clicked[2]);
        if (GUILayout.Button(new GUIContent("Physics", "Applies physics to the player in the direction that the player is facing")))
        {
            _type = typeof(ApplyPhysicsEventAsset);
            _clicked = new bool[] { false, false, true, false, false, false, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[1]);
        if (GUILayout.Button(new GUIContent("Items", "Spawns any item in the direction that the player is facing, as well as optionally letting the player hold it immediately")))
        {
            _type = typeof(SpawnItemEventAsset);
            _clicked = new bool[] { false, true, false, false, false, false, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(_clicked[4]);
        if (GUILayout.Button(new GUIContent("Visual FX", "Spawns a VFX element in the world based on some parameters")))
        {
            _type = typeof(SpawnVFXEventAsset);
            _clicked = new bool[] { false, false, false, false, true, false, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[5]);
        if (GUILayout.Button(new GUIContent("Audio Clip", "Plays an Audio Clip element in the world based on some parameters")))
        {
            _type = typeof(PlayClipEventAsset);
            _clicked = new bool[] { false, false, false, false, false, true, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(_clicked[7]);
        if (GUILayout.Button(new GUIContent("UnityEvent", "Executes any UnityEvent. This should only be used for client-side things, nothing that affects the game state!")))
        {
            _type = typeof(ExecuteUnityEventEventAsset);
            _clicked = new bool[] { false, false, false, false, false, false, false, true, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[6]);
        if (GUILayout.Button(new GUIContent("Hold While", "Allows the player to hold the animation while a condition is met. The number of frames the animation is held for is stored in a value which can be used by other events")))
        {
            _type = typeof(HoldAnimationEventAsset);
            _clicked = new bool[] { false, false, false, false, false, false, true, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        _beginningFrame = EditorGUILayout.IntField(_beginningFrame);
        _endingFrame = EditorGUILayout.IntField(_endingFrame);

        GUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(_clicked.All(item => !item));
        if (GUILayout.Button($"Create From Frames {_beginningFrame} - {_endingFrame}"))
        {
            FrameEventAsset asset;

            if (_type == typeof(PlayClipEventAsset))
                asset = AnimationEventContainer.CreateAsset<PlayClipEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(SpawnVFXEventAsset))
                asset = AnimationEventContainer.CreateAsset<SpawnVFXEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(SpawnItemEventAsset))
                asset = AnimationEventContainer.CreateAsset<SpawnItemEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(SetWeaponHitboxEventAsset))
                asset = AnimationEventContainer.CreateAsset<SetWeaponHitboxEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(SpawnDynamicHitboxEventAsset))
                asset = AnimationEventContainer.CreateAsset<SpawnDynamicHitboxEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(ModifyHurtboxesEventAsset))
                asset = AnimationEventContainer.CreateAsset<ModifyHurtboxesEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(ApplyPhysicsEventAsset))
                asset = AnimationEventContainer.CreateAsset<ApplyPhysicsEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(HoldAnimationEventAsset))
                asset = AnimationEventContainer.CreateAsset<HoldAnimationEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else if (_type == typeof(ExecuteUnityEventEventAsset))
                asset = AnimationEventContainer.CreateAsset<ExecuteUnityEventEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });
            else
                asset = AnimationEventContainer.CreateAsset<AllowMovementEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _beginningFrame;
                    t.Settings.EndingFrame = _endingFrame;
                });

            _events.Settings.Events.Add(new() { Id = asset.AssetObject.Guid });

            AnimationEventContainerWindow.Selected = asset;
            Selection.activeObject = AnimationEventContainerWindow.Selected;
        }

        EditorGUI.EndDisabledGroup();
    }
}
