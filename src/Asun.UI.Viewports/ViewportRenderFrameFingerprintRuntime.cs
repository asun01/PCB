using System.Security.Cryptography;
using System.Text;

namespace Asun.UI.Viewports;

public static class ViewportRenderFrameFingerprintRuntime
{
    public static string CreateFingerprint(
        ViewportRenderFrameSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        var canonical=string.Join("|",new[]
        {
            summary.Generation.ToString(),
            summary.CommandCount.ToString(),
            summary.RegionCount.ToString(),
            summary.TileCount.ToString(),
            summary.RoiCount.ToString(),
            summary.OverlayCount.ToString(),
            summary.InvalidationCount.ToString(),
            summary.FullSurfaceCount.ToString()
        });

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
