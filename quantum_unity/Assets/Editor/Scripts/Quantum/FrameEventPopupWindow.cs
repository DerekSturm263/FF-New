using System.Linq;
using UnityEditor;
using UnityEngine;
using static AnimationEventContainerWindow;

internal class FrameEventPopup : PopupWindowContent
{
    private int _frame;
    private QuantumAnimationEventAsset _events;
    private Object _parent;

    private string _name;
    private System.Type _type;
    private bool[] _clicked;

    public FrameEventPopup(int frame, QuantumAnimationEventAsset events, Object parent)
    {
        _frame = frame;
        _events = events;
        _parent = parent;
        _clicked = new bool[] { false, false, false, false, false, false };

        _name = "New Event";
    }

    public override void OnGUI(Rect rect)
    {
        EditorGUI.BeginDisabledGroup(_clicked[0]);
        if (GUILayout.Button("Spawn Hitbox"))
        {
            _type = typeof(SpawnHitboxEventAsset);
            _clicked = new bool[] { true, false, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[1]);
        if (GUILayout.Button("Spawn Projectile"))
        {
            _type = typeof(SpawnProjectileEventAsset);
            _clicked = new bool[] { false, true, false, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[2]);
        if (GUILayout.Button("Apply Physics"))
        {
            _type = typeof(ApplyPhysicsEventAsset);
            _clicked = new bool[] { false, false, true, false, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[3]);
        if (GUILayout.Button("Modify Hurtboxes"))
        {
            _type = typeof(ModifyHurtboxesEventAsset);
            _clicked = new bool[] { false, false, false, true, false, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[4]);
        if (GUILayout.Button("Spawn VFX"))
        {
            _type = typeof(SpawnVFXEventAsset);
            _clicked = new bool[] { false, false, false, false, true, false };
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(_clicked[5]);
        if (GUILayout.Button("Play Clip"))
        {
            _type = typeof(PlayClipEventAsset);
            _clicked = new bool[] { false, false, false, false, false, true };
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(15);

        _name = EditorGUILayout.TextField(_name);

        EditorGUI.BeginDisabledGroup(_clicked.All(item => !item));
        if (GUILayout.Button("Create"))
        {
            FrameEventAsset asset;

            if (_type == typeof(PlayClipEventAsset))
                asset = AnimationEventContainer.CreateAsset<PlayClipEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _frame;
                    t.Settings.EndingFrame = _frame + 1;
                });
            else if (_type == typeof(SpawnVFXEventAsset))
                asset = AnimationEventContainer.CreateAsset<SpawnVFXEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _frame;
                    t.Settings.EndingFrame = _frame + 1;
                });
            else if (_type == typeof(SpawnProjectileEventAsset))
                asset = AnimationEventContainer.CreateAsset<SpawnProjectileEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _frame;
                    t.Settings.EndingFrame = _frame + 1;
                });
            else if (_type == typeof(SpawnHitboxEventAsset))
                asset = AnimationEventContainer.CreateAsset<SpawnHitboxEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _frame;
                    t.Settings.EndingFrame = _frame + 1;
                    //t.Settings.Settings.Prototype = UnityDB.FindAssetForInspector();
                });
            else if (_type == typeof(ModifyHurtboxesEventAsset))
                asset = AnimationEventContainer.CreateAsset<ModifyHurtboxesEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _frame;
                    t.Settings.EndingFrame = _frame + 1;
                });
            else
                asset = AnimationEventContainer.CreateAsset<ApplyPhysicsEventAsset>(_name, _parent, t =>
                {
                    t.Settings.StartingFrame = _frame;
                    t.Settings.EndingFrame = _frame + 1;
                });

            _events.Settings.Events.Add(new() { Id = asset.AssetObject.Guid });

            Selected = asset;
            Selection.activeObject = Selected;
        }
        EditorGUI.EndDisabledGroup();
    }
}
