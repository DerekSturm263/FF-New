using Quantum;

[System.Serializable]
public struct UserProfile
{
    public float HapticStrength;
    public Build LastBuild;
    public void SetLastBuild(Build build) => LastBuild = build;

    public UserProfile(float hapticStrength, Build defaultBuild)
    {
        HapticStrength = hapticStrength;
        LastBuild = defaultBuild;
    }
}
