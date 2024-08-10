using Quantum;

[System.Serializable]
public struct UserProfile
{
    public float HapticStrength;
    public SerializableWrapper<Build> LastBuild;
    public void SetLastBuild(SerializableWrapper<Build> build) => LastBuild = build;

    public UserProfile(float hapticStrength, SerializableWrapper<Build> defaultBuild)
    {
        HapticStrength = hapticStrength;
        LastBuild = defaultBuild;
    }
}
