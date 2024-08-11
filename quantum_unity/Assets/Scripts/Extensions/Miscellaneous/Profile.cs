using Steamworks;
using UnityEngine;

namespace FusionFighters.Profile
{
    [System.Serializable]
    public struct Profile
    {
        [SerializeField] private Extensions.Types.Tuple<bool, string> _username;
        public readonly string Username => SteamManager.Initialized && _username.Item1 ? SteamFriends.GetPersonaName() : _username.Item2; 
        public void SetUsername(string username) => _username = new(false, username);

        [SerializeField] private Extensions.Types.Tuple<bool, Texture2D> _icon;
        public readonly Sprite Icon
        {
            get
            {
                Texture2D icon = null;

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

                            icon = avatarTexture;
                        }
                    }
                }

                if (!icon)
                    icon = _icon.Item2;

                try
                {
                    return Sprite.Create(icon, new(0, 0, -icon.width, -icon.height), Vector2.one);
                }
                catch
                {
                    return Sprite.Create(Texture2D.whiteTexture, new(0, 0, Texture2D.whiteTexture.width, -Texture2D.whiteTexture.height), Vector2.one);
                }
            }
        }
        public void SetIcon(Texture2D icon) => _icon = new(false, icon);

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

        public Profile DeepCopy()
        {
            return new()
            {
                _username = _username,
                _icon = _icon,
                _level = _level
            };
        }
    }
}
