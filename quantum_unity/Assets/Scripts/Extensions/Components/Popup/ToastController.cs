namespace GameResources.UI.Popup
{
    public class ToastController : SpawnableController<string>
    {
        protected override bool TakeAwayFocus() => false;

        protected override void SetUp(string message)
        {
            _templateInstance.GetComponentInChildren<TMPro.TMP_Text>().SetText(message);
        }
    }
}
