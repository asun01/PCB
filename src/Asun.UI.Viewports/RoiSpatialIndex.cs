using System.Numerics;
using System.Drawing;

namespace Asun.UI.Viewports;

public sealed class RoiSpatialIndex
{
    private readonly int _cellSize;
    private readonly Dictionary<(int X, int Y), List<RoiDocumentItem>> _cells = new();

    public RoiSpatialIndex(int cellSize = 128)
    {
        if (cellSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(cellSize));

        _cellSize = cellSize;
    }

    public int CellSize => _cellSize;

    public int CellCount => _cells.Count;

    public void Rebuild(IEnumerable<RoiDocumentItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _cells.Clear();

        foreach (var item in items)
        {
            if (!item.Geometry.IsValid)
                continue;

            Add(item);
        }
    }

    public void Add(RoiDocumentItem item)
    {
        if (!item.Geometry.IsValid)
            throw new ArgumentException("ROI geometry must be valid.", nameof(item));

        foreach (var cell in EnumerateCells(item.Geometry.GetBounds()))
        {
            if (!_cells.TryGetValue(cell, out var list))
            {
                list = new List<RoiDocumentItem>();
                _cells[cell] = list;
            }

            list.Add(item);
        }
    }

    public bool Remove(Guid id)
    {
        var removed = false;

        foreach (var pair in _cells.ToArray())
        {
            var beforeCount = pair.Value.Count;
            pair.Value.RemoveAll(item => item.Id == id);
            removed |= pair.Value.Count != beforeCount;

            if (pair.Value.Count == 0)
                _cells.Remove(pair.Key);
        }

        return removed;
    }

    public IReadOnlyList<RoiDocumentItem> QueryPoint(Vector2 point)
    {
        Validate(point);

        var cell = GetCell(point);
        if (!_cells.TryGetValue(cell, out var candidates))
            return Array.Empty<RoiDocumentItem>();

        return candidates
            .Where(item => item.Geometry.Contains(point))
            .OrderByDescending(item => item.ZIndex)
            .ThenBy(item => item.Id)
            .ToArray();
    }

    public IReadOnlyList<RoiDocumentItem> QueryRectangle(RectangleF rectangle)
    {
        if (!IsFiniteRectangle(rectangle) || rectangle.Width < 0 || rectangle.Height < 0)
            throw new ArgumentOutOfRangeException(nameof(rectangle));

        var result = new Dictionary<Guid, RoiDocumentItem>();

        foreach (var cell in EnumerateCells(rectangle))
        {
            if (!_cells.TryGetValue(cell, out var candidates))
                continue;

            foreach (var item in candidates)
            {
                if (item.Geometry.GetBounds().IntersectsWith(rectangle))
                    result[item.Id] = item;
            }
        }

        return result.Values
            .OrderByDescending(item => item.ZIndex)
            .ThenBy(item => item.Id)
            .ToArray();
    }

    public RoiDocumentHit HitTest(
        Vector2 point,
        float handleTolerance = 8f,
        float bodyTolerance = 0f)
    {
        Validate(point);

        var cell = GetCell(point);
        if (!_cells.TryGetValue(cell, out var candidates))
            return new RoiDocumentHit(Guid.Empty, RoiHitResult.None);

        foreach (var item in candidates
                     .OrderByDescending(item => item.ZIndex)
                     .ThenBy(item => item.Id))
        {
            var hit = RoiHitTester.HitTest(
                item.Geometry,
                point,
                handleTolerance,
                bodyTolerance);

            if (hit.Hit)
                return new RoiDocumentHit(item.Id, hit);
        }

        return new RoiDocumentHit(Guid.Empty, RoiHitResult.None);
    }

    private (int X, int Y) GetCell(Vector2 point) =>
        ((int)MathF.Floor(point.X / _cellSize),
         (int)MathF.Floor(point.Y / _cellSize));

    private IEnumerable<(int X, int Y)> EnumerateCells(RectangleF rectangle)
    {
        var minX = (int)MathF.Floor(rectangle.Left / _cellSize);
        var maxX = (int)MathF.Floor(rectangle.Right / _cellSize);
        var minY = (int)MathF.Floor(rectangle.Top / _cellSize);
        var maxY = (int)MathF.Floor(rectangle.Bottom / _cellSize);

        for (var y = minY; y <= maxY; y++)
        for (var x = minX; x <= maxX; x++)
            yield return (x, y);
    }

    private static bool IsFiniteRectangle(RectangleF rectangle) =>
        float.IsFinite(rectangle.X) &&
        float.IsFinite(rectangle.Y) &&
        float.IsFinite(rectangle.Width) &&
        float.IsFinite(rectangle.Height);

    private static void Validate(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));
    }
}
