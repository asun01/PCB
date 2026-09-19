namespace Asun.UI.Viewports;

public static class RoiLockRuntime
{
    public static IReadOnlySet<Guid> LockedIds(RoiLayerRuntime layers) =>
        layers.Snapshot().Where(x => x.Locked).Select(x => x.RoiId).ToHashSet();
}
