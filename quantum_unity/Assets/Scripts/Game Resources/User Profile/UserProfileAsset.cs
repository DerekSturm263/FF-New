using UnityEngine;

[CreateAssetMenu(fileName = "New User Profile", menuName = "Fusion Fighters/User Profile")]
public class UserProfileAsset : InfoAssetAsset
{
    public SerializableWrapper<UserProfile> Profile;
}
