using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Asun.UI.Viewports;

/// <summary>
/// Produces deterministic, backend-neutral evidence fingerprints for viewport
/// render artifacts. The fingerprint describes the logical render contract only;
/// it is not a pixel hash and is not a substitute for production image golden data.
/// </summary>
public static class ViewportRenderEvidenceRuntime
{
    public static string ComputeCommandStreamHash(
        ViewportRenderCommandStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var builder = new StringBuilder();
        Append(builder, "generation", stream.Generation);
        Append(builder, "commands", stream.Commands.Count);

        foreach (var command in stream.Commands)
        {
            Append(builder, "sequence", command.Sequence);
            Append(builder, "kind", command.Kind);
            Append(builder, "bounds", command.Bounds);

            var work = command.WorkItem;
            Append(builder, "work.kind", work.Kind);
            Append(builder, "work.roi", work.RoiId);
            Append(builder, "work.tile", work.Tile?.ToString() ?? "-");
            Append(builder, "work.generation", work.Generation);
            Append(builder, "work.invalidation", work.IsInvalidation);
        }

        return Hash(builder);
    }

    public static string ComputeBatchHash(
        ViewportRenderBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        var builder = new StringBuilder();
        Append(builder, "generation", batch.Generation);

        foreach (var item in batch.Items)
        {
            Append(builder, "kind", item.Kind);
            Append(builder, "bounds", item.Bounds);
            Append(builder, "roi", item.RoiId);
            Append(builder, "tile", item.Tile?.ToString() ?? "-");
            Append(builder, "generation", item.Generation);
            Append(builder, "invalidation", item.IsInvalidation);
        }

        foreach (var region in batch.Regions)
            Append(builder, "region", region);

        return Hash(builder);
    }

    public static string ComputePipelineFrameHash<TTile>(
        ViewportRenderPipelineFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var builder = new StringBuilder();
        Append(builder, "composite.generation", frame.Composite.Generation);
        Append(builder, "submission.sequence", frame.Submission.Sequence);
        Append(builder, "submission.flags", frame.Submission.DirtyFlags);
        Append(builder, "submission.generation", frame.Submission.Generation);
        Append(builder, "work.generation", frame.WorkPlan.Generation);
        Append(builder, "batch.hash", ComputeBatchHash(frame.Batch));
        Append(builder, "command.hash", ComputeCommandStreamHash(frame.CommandStream));
        Append(builder, "deferred", frame.HasDeferredWork);

        return Hash(builder);
    }

    public static string ComputeReplayHash(
        IReadOnlyList<ViewportRenderReplayOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);

        var builder = new StringBuilder();

        foreach (var operation in operations)
        {
            Append(builder, "sequence", operation.Sequence);
            Append(builder, "kind", operation.Kind);
            Append(builder, "generation", operation.Generation);
            Append(builder, "tile", operation.Tile?.ToString() ?? "-");
            Append(builder, "roi", operation.RoiId);
            Append(builder, "bounds", operation.Bounds);
            Append(builder, "rendered", operation.RenderedUnits);
            Append(
                builder,
                "status",
                operation.DeliveryStatus?.ToString() ?? "-");
        }

        return Hash(builder);
    }

    public static string ComputeTextHash(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return Hash(new StringBuilder(text));
    }

    private static string Hash(StringBuilder builder) =>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())));

    private static void Append(
        StringBuilder builder,
        string key,
        object? value)
    {
        builder
            .Append(key)
            .Append('=')
            .Append(value switch
            {
                null => "-",
                float single => single.ToString("R", CultureInfo.InvariantCulture),
                double number => number.ToString("R", CultureInfo.InvariantCulture),
                Guid guid => guid.ToString("N"),
                _ => value.ToString()
            })
            .Append(';');
    }

    private static void Append(
        StringBuilder builder,
        string key,
        System.Drawing.RectangleF value)
    {
        builder
            .Append(key)
            .Append('=')
            .Append(BitConverter.SingleToInt32Bits(value.X))
            .Append(',')
            .Append(BitConverter.SingleToInt32Bits(value.Y))
            .Append(',')
            .Append(BitConverter.SingleToInt32Bits(value.Width))
            .Append(',')
            .Append(BitConverter.SingleToInt32Bits(value.Height))
            .Append(';');
    }
}
