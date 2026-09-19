namespace Asun.UI.Viewports;

public sealed record ViewportReplaySessionBundle(
    ViewportReplaySessionManifest Manifest,
    ViewportInputEvent[] Inputs,
    ViewportRenderEvidenceManifest[] Evidence,
    ViewportPresentationAuditEvent[] Audit)
{
    public int FormatVersion { get; init; } =
        ViewportReplaySessionBundleRuntime.CurrentFormatVersion;

    public bool IsEmpty =>
        Manifest.IsEmpty &&
        Inputs is { Length: 0 } &&
        Evidence is { Length: 0 } &&
        Audit is { Length: 0 };
}
