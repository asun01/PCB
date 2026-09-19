using System.Numerics;
using System.Text.Json;

namespace Asun.UI.Viewports;

public sealed class RoiClipboardRuntime
{
    private sealed record ShapeDto(RoiShapeKind Kind, Vector2 Center, Vector2 Size, float Rotation, Vector2[] Vertices);
    private readonly object _sync = new();
    private string? _payload;

    public bool HasData { get { lock (_sync) return !string.IsNullOrEmpty(_payload); } }

    public void Copy(IEnumerable<RoiDocumentItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var data = items.Select(item => new
        {
            item.Id,
            item.ZIndex,
            Shape = new ShapeDto(
                item.Geometry.Kind,
                item.Geometry.Center,
                item.Geometry.Size,
                item.Geometry.RotationRadians,
                item.Geometry.Vertices.ToArray())
        }).ToArray();
        lock (_sync) _payload = JsonSerializer.Serialize(data);
    }

    public IReadOnlyList<RoiDocumentItem> Paste(Vector2 offset = default)
    {
        Validate(offset);
        string? payload;
        lock (_sync) payload = _payload;
        if (string.IsNullOrEmpty(payload)) return Array.Empty<RoiDocumentItem>();

        using var document = JsonDocument.Parse(payload);
        var result = new List<RoiDocumentItem>();
        foreach (var element in document.RootElement.EnumerateArray())
        {
            var shape = element.GetProperty("Shape");
            var kind = Enum.Parse<RoiShapeKind>(shape.GetProperty("Kind").GetString()!);
            var center = shape.GetProperty("Center").Deserialize<Vector2>();
            var size = shape.GetProperty("Size").Deserialize<Vector2>();
            var rotation = shape.GetProperty("Rotation").GetSingle();
            var vertices = shape.GetProperty("Vertices").Deserialize<Vector2[]>() ?? Array.Empty<Vector2>();
            var geometry = kind switch
            {
                RoiShapeKind.Rectangle => RoiGeometry.CreateRectangle(center, size),
                RoiShapeKind.RotatedRectangle => RoiGeometry.CreateRotatedRectangle(center, size, rotation),
                RoiShapeKind.Ellipse => RoiGeometry.CreateEllipse(center, size, rotation),
                RoiShapeKind.Polygon => RoiGeometry.CreatePolygon(vertices),
                _ => throw new InvalidOperationException()
            };
            geometry = geometry.Translate(offset);
            result.Add(new RoiDocumentItem(Guid.NewGuid(), geometry, result.Count));
        }
        return result;
    }

    public void Clear() { lock (_sync) _payload = null; }

    private static void Validate(Vector2 value)
    {
        if (!float.IsFinite(value.X) || !float.IsFinite(value.Y))
            throw new ArgumentOutOfRangeException(nameof(value));
    }
}
