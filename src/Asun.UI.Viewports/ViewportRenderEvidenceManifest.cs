using System.Text.Json;

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
            new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
    }
}
