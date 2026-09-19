namespace Asun.UI.Viewports;

public sealed record ViewportReplaySessionBundle(
    ViewportReplaySessionManifest Manifest,
    ViewportInputEvent[] Inputs,
    ViewportRenderEvidenceManifest[] Evidence,
    ViewportPresentationAuditEvent[] Audit)
{
    public bool IsEmpty =>
        Manifest.IsEmpty &&
        Inputs.Length == 0 &&
        Evidence.Length == 0 &&
        Audit.Length == 0;
}
