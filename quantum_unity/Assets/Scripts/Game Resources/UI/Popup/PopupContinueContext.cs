using System;

namespace GameResources.UI.Popup
{
    public class PopupContinueContext
    {
        private Action _previousAction;
        public void InvokePreviousAction() => _previousAction?.Invoke();

        public PopupContinueContext(Action previousAction)
        {
            _previousAction = previousAction;
        }
    }
}
