using System.Numerics;

namespace Asun.UI.Viewports;

public static class RoiMultiSelectionTransformRuntime
{
    public static IReadOnlyList<RoiDocumentItem> Translate(IEnumerable<RoiDocumentItem> items, Vector2 delta) =>
        RoiGroupTransformRuntime.Translate(items, delta).Items;

    public static IReadOnlyList<RoiDocumentItem> Rotate(
        IEnumerable<RoiDocumentItem> items, float radians, Vector2? pivot = null) =>
        RoiGroupTransformRuntime.Rotate(items, radians, pivot).Items;

    public static IReadOnlyList<RoiDocumentItem> Scale(
        IEnumerable<RoiDocumentItem> items, Vector2 factors, Vector2? pivot = null) =>
        RoiGroupTransformRuntime.Scale(items, factors, pivot).Items;
}
