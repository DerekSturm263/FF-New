using Quantum;

public static class FrameBaseExtensions
{
    public enum SpaceType
    {
        TwoD,
        ThreeD
    }

    public static EntityRef CreateChilded(this Frame f, AssetRefEntityPrototype prototype, EntityRef parent)
    {
        EntityRef entity = f.Create(prototype);

        ChildParentLink link = new()
        {
            Parent = parent
        };
        f.Add(entity, link);

        return entity;
    }
}
