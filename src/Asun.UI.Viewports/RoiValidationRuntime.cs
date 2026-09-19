using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiValidationIssue(string Code, string Message);

public static class RoiValidationRuntime
{
    public static IReadOnlyList<RoiValidationIssue> Validate(
        IEnumerable<RoiDocumentItem> items,
        Vector2 imageSize)
    {
        ArgumentNullException.ThrowIfNull(items);
        var issues = new List<RoiValidationIssue>();
        if (!float.IsFinite(imageSize.X) || !float.IsFinite(imageSize.Y) || imageSize.X <= 0 || imageSize.Y <= 0)
            issues.Add(new("IMAGE_SIZE", "Image size is invalid."));

        foreach (var item in items)
        {
            if (!item.Geometry.IsValid)
                issues.Add(new("INVALID_GEOMETRY", $"ROI {item.Id} geometry is invalid."));
            if (item.Geometry.GetBounds().Width <= 0 || item.Geometry.GetBounds().Height <= 0)
                issues.Add(new("EMPTY_GEOMETRY", $"ROI {item.Id} has empty bounds."));
        }

        return issues;
    }
}
