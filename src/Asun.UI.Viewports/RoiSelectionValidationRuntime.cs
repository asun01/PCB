using System.Drawing;

namespace Asun.UI.Viewports;

public static class RoiSelectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        RoiSelectionSnapshot snapshot,
        IReadOnlySet<Guid> knownIds)
    {
        ArgumentNullException.ThrowIfNull(knownIds);

        var errors = new List<string>();

        if (snapshot.SelectedIds.Any(id =>
            id == Guid.Empty || !knownIds.Contains(id)))
        {
            errors.Add("Selection contains an unknown or empty ROI id.");
        }

        if (snapshot.IsMarqueeActive)
        {
            if (snapshot.Marquee is not RectangleF marquee ||
                !IsFinite(marquee) ||
                marquee.Width < 0 ||
                marquee.Height < 0)
            {
                errors.Add("Active marquee must carry a finite normalized rectangle.");
            }
        }
        else if (snapshot.Marquee is not null)
        {
            errors.Add("Inactive selection must not expose an active marquee rectangle.");
        }

        return errors;
    }

    public static bool IsValid(
        RoiSelectionSnapshot snapshot,
        IReadOnlySet<Guid> knownIds) =>
        Validate(snapshot, knownIds).Count == 0;

    private static bool IsFinite(RectangleF rectangle) =>
        float.IsFinite(rectangle.X) &&
        float.IsFinite(rectangle.Y) &&
        float.IsFinite(rectangle.Width) &&
        float.IsFinite(rectangle.Height);
}
