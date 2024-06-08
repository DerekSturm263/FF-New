using FusionFighters.Profile;
using System.Linq;
using UnityEngine;

namespace Quantum.Demo
{
    public class UIConnect : UIScreen<UIConnect>
    {
        public PhotonRegions SelectableRegions;
        public PhotonAppVersions SelectableAppVersion;

        public string Region;

        protected new void Awake()
        {
            base.Awake();
        }

        public override void OnShowScreen(bool first)
        {
            base.OnShowScreen(first);
        }

        public void OnConnectClicked()
        {
            if (string.IsNullOrEmpty(Profile.Instance.Username.Trim()))
            {
                UIDialog.Show("Error", "User name not set.");
                return;
            }

            var appSettings = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);
            UIMain.Client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);

            // Overwrite region
            if (string.IsNullOrEmpty(appSettings.Server) == false)
            {
                // Direct connect will not set a region
                appSettings.FixedRegion = string.Empty;
            }
            else
            {
                // Connections to nameserver require an app id
                if (string.IsNullOrEmpty(appSettings.AppIdRealtime.Trim()))
                {
                    UIDialog.Show("Error", "AppId not set.\n\nSearch or create PhotonServerSettings and configure an AppId.");
                    return;
                }

                appSettings.FixedRegion = SelectableRegions.Regions.First(item => item.Name == Region).Token;
            }

            // Append selected app version
            appSettings.AppVersion += PhotonAppVersions.AppendAppVersion(PhotonAppVersions.Type.UsePhotonAppVersion, SelectableAppVersion);

            if (UIMain.Client.ConnectUsingSettings(appSettings, Profile.Instance.Username))
            {
                HideScreen();
                UIConnecting.ShowScreen();
            }
            else
            {
                Debug.LogError($"Failed to connect with app settings: '{appSettings.ToStringFull()}'");
            }
        }
    }
}
