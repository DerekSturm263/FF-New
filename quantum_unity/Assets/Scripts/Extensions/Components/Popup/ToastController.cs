namespace GameResources.UI.Popup
{
    public class ToastController : SpawnableController<string>
    {
        protected override bool TakeAwayFocus() => false;
        protected override bool CanSpawn => _activeCount == 0;

        protected override void SetUp(string message)
        {
            _templateInstance.GetComponentInChildren<TMPro.TMP_Text>().SetText(message);
        }

        public void DecreaseActiveCount() => --_activeCount;
    }
}
