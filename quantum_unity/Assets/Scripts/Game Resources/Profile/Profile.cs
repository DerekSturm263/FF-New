using Steamworks;
using UnityEngine;

namespace FusionFighters.Profile
{
    [System.Serializable]
    public struct Profile
    {
        public static Profile Default = new()
        {
            _level = 1,
            _username = new(true, "New Player"),
            _icon = new(true, Texture2D.whiteTexture),
        };

        private static Profile _instance;
        public static ref Profile Instance
        {
            get
            {
                if (Serializer.TryLoadAs($"{Application.persistentDataPath}/SaveData/Misc/Profile.json", $"{Application.persistentDataPath}/SaveData/Misc", out Profile profile))
                    _instance = profile;
                else
                    _instance = Default;

                return ref _instance;
            }
        }

        [SerializeField] private Extensions.Types.Tuple<bool, string> _username;
        public string Username => SteamManager.Initialized && _username.Item1 ? SteamFriends.GetPersonaName() : _username.Item2; 
        public void SetUsername(string username) => _username = new(false, username);

        [SerializeField] private Extensions.Types.Tuple<bool, Texture2D> _icon;
        public Texture2D Icon
        {
            get
            {
                if (SteamManager.Initialized && _icon.Item1)
                {
                    int avatarID = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
                    if (avatarID != 0)
                    {
                        byte[] colors = new byte[184 * 184 * 4];

                        if (SteamUtils.GetImageRGBA(avatarID, colors, colors.Length))
                        {
                            Texture2D avatarTexture = new(184, 184, TextureFormat.RGBA32, false, true);

                            avatarTexture.LoadRawTextureData(colors);
                            avatarTexture.Apply();

                            return avatarTexture;
                        }
                    }
                }
                
                return _icon.Item2;
            }
        }
        public void SetIcon(Texture2D icon) => _icon = new(false, icon);

        /*[SerializeField] private IconFrame _iconFrame;
        public IconFrame IconFrame => _iconFrame;

        [SerializeField] private CardBackground _cardBackground;
        public CardBackground CardBackground => _cardBackground;

        [SerializeField] private List<Extensions.Types.Positional<Stamp>> _stamps;
        public List<Extensions.Types.Positional<Stamp>> Stamps => _stamps;*/

        [SerializeField] private float _level;
        public int Level => (int)_level;
        public float Experience => _level - Level;

        public void GainExperience(float amount)
        {
            int oldLevel = (int)_level;
            _level += amount;
            int newLevel = (int)_level;

            /*if (oldLevel != newLevel)
                user.InvokeOnLevelUp(oldLevel, newLevel);*/
        }

        public static string FilePath => $"{Application.persistentDataPath}";
    }
}
