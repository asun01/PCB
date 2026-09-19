using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderEvidenceManifest(
    long Generation,
    long SubmissionSequence,
    ViewportDirtyFlags DirtyFlags,
    int BatchItemCount,
    int RegionCount,
    int TileCount,
    int RoiCount,
    int OverlayCount,
    int InvalidationCount,
    int FullSurfaceCount,
    int PlannedUnits,
    int RenderedUnits,
    int DeferredUnits,
    string BatchHash,
    string CommandHash,
    string FrameHash,
    string ReplayHash)
{
    public bool IsComplete =>
        DeferredUnits == 0;

    public string StableKey =>
        $"{Generation}:{SubmissionSequence}:{FrameHash}";

    public string ToJson()
    {
        return JsonSerializer.Serialize(
            this,
            ViewportRenderEvidenceJsonContext.Default.ViewportRenderEvidenceManifest);
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ViewportRenderEvidenceManifest))]
internal partial class ViewportRenderEvidenceJsonContext : JsonSerializerContext
{
}
