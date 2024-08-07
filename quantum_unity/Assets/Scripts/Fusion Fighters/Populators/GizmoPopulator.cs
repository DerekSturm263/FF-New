public class GizmoPopulator : PopulateAsset<GizmoAsset>
{
    protected override string FilePath() => "DB/Assets/Stage/Gizmos";

    protected override bool IsEquipped(GizmoAsset item) => false;
}
