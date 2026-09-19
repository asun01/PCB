using System.Numerics;

namespace Asun.UI.Viewports;

public sealed record ViewportReplayStateFingerprint(
    ViewportTransform Transform,
    RoiDocumentSnapshot RoiDocument,
    long Generation,
    string StateHash)
{
    public int RoiCount => RoiDocument.Items.Count;

    public Guid? SelectedRoiId => RoiDocument.SelectedId;

    public RoiEditorMode RoiMode => RoiDocument.Mode;
}

public readonly record struct ViewportReplayStateComparison(
    bool IsEquivalent,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayStateComparison Equivalent { get; } =
        new(true, Array.Empty<string>());
}

public static class ViewportReplayStateFingerprintRuntime
{
    public static ViewportReplayStateFingerprint Capture<TTile>(
        ViewportCompositeRuntime<TTile> composite)
    {
        ArgumentNullException.ThrowIfNull(composite);

        var transform = composite.Transform;
        var roi = composite.RoiRuntime.Document.CreateSnapshot();
        var generation = composite.Generation;

        var stateText = BuildStateText(
            transform,
            roi,
            generation);

        return new ViewportReplayStateFingerprint(
            transform,
            roi,
            generation,
            ViewportRenderEvidenceRuntime.ComputeTextHash(stateText));
    }

    public static ViewportReplayStateComparison Compare(
        ViewportReplayStateFingerprint expected,
        ViewportReplayStateFingerprint actual)
    {
        var differences = new List<string>();

        CompareValue(
            differences,
            "Transform.Scale",
            expected.Transform.Scale,
            actual.Transform.Scale);
        CompareValue(
            differences,
            "Transform.Translation",
            expected.Transform.Translation,
            actual.Transform.Translation);
        CompareValue(
            differences,
            "Transform.ImageSize",
            expected.Transform.ImageSize,
            actual.Transform.ImageSize);
        CompareValue(
            differences,
            "Transform.ViewportSize",
            expected.Transform.ViewportSize,
            actual.Transform.ViewportSize);
        CompareValue(
            differences,
            "Generation",
            expected.Generation,
            actual.Generation);
        CompareValue(
            differences,
            "Roi.SelectedId",
            expected.RoiDocument.SelectedId,
            actual.RoiDocument.SelectedId);
        CompareValue(
            differences,
            "Roi.Mode",
            expected.RoiDocument.Mode,
            actual.RoiDocument.Mode);
        CompareValue(
            differences,
            "Roi.Count",
            expected.RoiDocument.Items.Count,
            actual.RoiDocument.Items.Count);

        var count = Math.Min(
            expected.RoiDocument.Items.Count,
            actual.RoiDocument.Items.Count);

        for (var index = 0; index < count; index++)
        {
            var left = expected.RoiDocument.Items[index];
            var right = actual.RoiDocument.Items[index];

            CompareValue(
                differences,
                $"Roi.Items[{index}].Id",
                left.Id,
                right.Id);
            CompareValue(
                differences,
                $"Roi.Items[{index}].ZIndex",
                left.ZIndex,
                right.ZIndex);
            CompareGeometry(
                differences,
                $"Roi.Items[{index}].Geometry",
                left.Geometry,
                right.Geometry);
        }

        CompareValue(
            differences,
            "StateHash",
            expected.StateHash,
            actual.StateHash);

        return differences.Count == 0
            ? ViewportReplayStateComparison.Equivalent
            : new ViewportReplayStateComparison(false, differences);
    }

    public static bool AreEquivalent(
        ViewportReplayStateFingerprint expected,
        ViewportReplayStateFingerprint actual) =>
        Compare(expected, actual).IsEquivalent;

    private static string BuildStateText(
        ViewportTransform transform,
        RoiDocumentSnapshot roi,
        long generation)
    {
        var roiText = string.Join(
            "\n",
            roi.Items.Select(item =>
                $"{item.Id:D}|{item.ZIndex}|{GeometryText(item.Geometry)}"));

        return string.Join(
            "|",
            generation,
            $"{transform.Scale:R}",
            $"{transform.Translation.X:R}",
            $"{transform.Translation.Y:R}",
            $"{transform.ImageSize.X:R}",
            $"{transform.ImageSize.Y:R}",
            $"{transform.ViewportSize.X:R}",
            $"{transform.ViewportSize.Y:R}",
            roi.SelectedId?.ToString("D") ?? string.Empty,
            roi.Mode,
            roiText);
    }

    private static string GeometryText(RoiGeometry geometry)
    {
        var vertices = string.Join(
            ";",
            geometry.Vertices.Select(
                point => $"{point.X:R},{point.Y:R}"));

        return string.Join(
            "|",
            geometry.Kind,
            $"{geometry.Center.X:R}",
            $"{geometry.Center.Y:R}",
            $"{geometry.Size.X:R}",
            $"{geometry.Size.Y:R}",
            $"{geometry.RotationRadians:R}",
            vertices);
    }

    private static void CompareGeometry(
        ICollection<string> differences,
        string path,
        RoiGeometry expected,
        RoiGeometry actual)
    {
        CompareValue(
            differences,
            $"{path}.Kind",
            expected.Kind,
            actual.Kind);
        CompareValue(
            differences,
            $"{path}.Center",
            expected.Center,
            actual.Center);
        CompareValue(
            differences,
            $"{path}.Size",
            expected.Size,
            actual.Size);
        CompareValue(
            differences,
            $"{path}.RotationRadians",
            expected.RotationRadians,
            actual.RotationRadians);

        CompareValue(
            differences,
            $"{path}.Vertices.Count",
            expected.Vertices.Count,
            actual.Vertices.Count);

        var count = Math.Min(
            expected.Vertices.Count,
            actual.Vertices.Count);

        for (var index = 0; index < count; index++)
        {
            CompareValue(
                differences,
                $"{path}.Vertices[{index}]",
                expected.Vertices[index],
                actual.Vertices[index]);
        }
    }

    private static void CompareValue(
        ICollection<string> differences,
        string path,
        object? expected,
        object? actual)
    {
        if (!Equals(expected, actual))
        {
            differences.Add(
                $"{path}: expected '{expected}', actual '{actual}'.");
        }
    }
}
