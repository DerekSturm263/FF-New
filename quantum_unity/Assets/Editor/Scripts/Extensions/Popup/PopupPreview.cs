using UnityEditor;
using UnityEngine;

using GRPopup = GameResources.UI.Popup.Popup;

namespace Editor.GameResources.UI.Popup
{
    [CustomPreview(typeof(GRPopup))]
    internal class PopupPreview : ObjectPreview
    {
        public override void Initialize(Object[] targets)
        {
            base.Initialize(targets);
        }

        public override bool HasPreviewGUI() => true;

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
        }

        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
        }

        public override string GetInfoString() => "Test";
    }
}
