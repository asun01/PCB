using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbExecutionIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record PcbExecutionRoiReplayBinding(
    Guid ProductionSessionId,
    Guid QualityRunId,
    string PcbExecutionFingerprint,
    string RoiBindingFingerprint,
    string ReplayConvergenceFingerprint,
    bool ReleaseReady,
    string ArtifactPath,
    string BindingFingerprint);

public static class PcbExecutionRoiReplayBindingRuntime
{
    public static PcbExecutionRoiReplayBinding Create(
        PcbExecutionRoiContextBinding pcbRoiBinding,
        ProductionCapturePcbAuditReplayConvergence replayConvergence)
    {
        ArgumentNullException.ThrowIfNull(pcbRoiBinding);
        ArgumentNullException.ThrowIfNull(replayConvergence);

        var errors=Validate(pcbRoiBinding,replayConvergence);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            pcbRoiBinding.ProductionSessionId,
            pcbRoiBinding.QualityRunId,
            pcbRoiBinding.PcbExecutionFingerprint,
            pcbRoiBinding.RoiBindingFingerprint,
            replayConvergence.Fingerprint,
            replayConvergence.ReleaseManifestFingerprint,
            replayConvergence.ReleaseReady,
            replayConvergence.ArtifactPath);

        return new PcbExecutionRoiReplayBinding(
            pcbRoiBinding.ProductionSessionId,
            pcbRoiBinding.QualityRunId,
            pcbRoiBinding.PcbExecutionFingerprint,
            pcbRoiBinding.RoiBindingFingerprint,
            replayConvergence.Fingerprint,
            replayConvergence.ReleaseReady,
            replayConvergence.ArtifactPath,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        PcbExecutionRoiContextBinding pcbRoiBinding,
        ProductionCapturePcbAuditReplayConvergence replayConvergence)
    {
        ArgumentNullException.ThrowIfNull(pcbRoiBinding);
        ArgumentNullException.ThrowIfNull(replayConvergence);

        var errors=new List<string>();
        if(!PcbExecutionRoiContextBindingRuntime.IsValidBinding(pcbRoiBinding))
            errors.Add("PCB execution/ROI binding is invalid.");

        if(replayConvergence.ProductionSessionId==Guid.Empty)
            errors.Add("Replay convergence Production session id cannot be empty.");
        if(replayConvergence.QualityRunId==Guid.Empty)
            errors.Add("Replay convergence Quality run id cannot be empty.");

        foreach(var pair in new[]
        {
            (nameof(replayConvergence.ReleaseManifestFingerprint),replayConvergence.ReleaseManifestFingerprint),
            (nameof(replayConvergence.CaptureAuditReplayFingerprint),replayConvergence.CaptureAuditReplayFingerprint),
            (nameof(replayConvergence.PcbAuditReplayClosureFingerprint),replayConvergence.PcbAuditReplayClosureFingerprint),
            (nameof(replayConvergence.Fingerprint),replayConvergence.Fingerprint)
        })
        {
            if(pair.Item2.Length!=64 ||
               !pair.Item2.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
                errors.Add(pair.Item1+" must be 64 lowercase hexadecimal characters.");
        }

        if(string.IsNullOrWhiteSpace(replayConvergence.ArtifactPath))
            errors.Add("Replay convergence logical artifact path cannot be blank.");

        if(pcbRoiBinding.ProductionSessionId!=replayConvergence.ProductionSessionId)
            errors.Add("PCB/ROI and replay convergence Production session identities must match.");
        if(pcbRoiBinding.QualityRunId!=replayConvergence.QualityRunId)
            errors.Add("PCB/ROI and replay convergence Quality identities must match.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionRoiContextBinding pcbRoiBinding,
        ProductionCapturePcbAuditReplayConvergence replayConvergence)=>
        Validate(pcbRoiBinding,replayConvergence).Count==0;

    public static bool IsEquivalent(
        PcbExecutionRoiReplayBinding left,
        PcbExecutionRoiReplayBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    public static bool IsReleaseReady(
        PcbExecutionRoiReplayBinding binding)=>
        binding.ReleaseReady;

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
