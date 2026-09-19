using System.Numerics;

namespace Asun.UI.Viewports;

public static class RoiDuplicatePatternRuntime
{
    public static IReadOnlyList<RoiDocumentItem> Grid(
        RoiDocumentItem source, int columns, int rows, Vector2 spacing)
    {
        if (columns <= 0 || rows <= 0) throw new ArgumentOutOfRangeException();
        Validate(spacing);
        var list = new List<RoiDocumentItem>(columns * rows);
        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns; x++)
            list.Add(source with
            {
                Id = Guid.NewGuid(),
                ZIndex = list.Count,
                Geometry = source.Geometry.Translate(new Vector2(x * spacing.X, y * spacing.Y))
            });
        return list;
    }

    public static IReadOnlyList<RoiDocumentItem> Linear(
        RoiDocumentItem source, int count, Vector2 delta)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        Validate(delta);
        return Enumerable.Range(0, count)
            .Select(i => source with
            {
                Id = Guid.NewGuid(),
                ZIndex = i,
                Geometry = source.Geometry.Translate(delta * i)
            })
            .ToArray();
    }

    private static void Validate(Vector2 v)
    {
        if (!float.IsFinite(v.X) || !float.IsFinite(v.Y)) throw new ArgumentOutOfRangeException();
    }
}
