using System.Security.Cryptography;
using System.Text;
using Asun.Platform.QualityReleaseIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record PcbExecutionRoiQualityReleaseReplayClosure(
    Guid ProductionSessionId,
    Guid QualityRunId,
    string RoiBindingFingerprint,
    string ReplayConvergenceFingerprint,
    bool ReleaseReady,
    string ArtifactPath,
    string QualitySummaryFingerprint,
    string ReleaseManifestFingerprint,
    string QualityReleaseDescriptorFingerprint,
    string ClosureFingerprint);

public static class PcbExecutionRoiQualityReleaseReplayClosureRuntime
{
    public static PcbExecutionRoiQualityReleaseReplayClosure Create(
        PcbExecutionRoiReplayBinding roiReplayBinding,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor)
    {
        ArgumentNullException.ThrowIfNull(roiReplayBinding);
        ArgumentNullException.ThrowIfNull(qualityReleaseDescriptor);

        var errors=Validate(roiReplayBinding,qualityReleaseDescriptor);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            roiReplayBinding.ProductionSessionId,
            roiReplayBinding.QualityRunId,
            roiReplayBinding.RoiBindingFingerprint,
            roiReplayBinding.ReplayConvergenceFingerprint,
            roiReplayBinding.ReleaseReady,
            roiReplayBinding.ArtifactPath,
            qualityReleaseDescriptor.QualityRunId,
            qualityReleaseDescriptor.QualitySummaryFingerprint,
            qualityReleaseDescriptor.ReleaseManifestFingerprint,
            qualityReleaseDescriptor.ReleaseReady,
            qualityReleaseDescriptor.DescriptorFingerprint);

        return new PcbExecutionRoiQualityReleaseReplayClosure(
            roiReplayBinding.ProductionSessionId,
            roiReplayBinding.QualityRunId,
            roiReplayBinding.RoiBindingFingerprint,
            roiReplayBinding.ReplayConvergenceFingerprint,
            roiReplayBinding.ReleaseReady,
            roiReplayBinding.ArtifactPath,
            qualityReleaseDescriptor.QualitySummaryFingerprint,
            qualityReleaseDescriptor.ReleaseManifestFingerprint,
            qualityReleaseDescriptor.DescriptorFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        PcbExecutionRoiReplayBinding roiReplayBinding,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor)
    {
        ArgumentNullException.ThrowIfNull(roiReplayBinding);
        ArgumentNullException.ThrowIfNull(qualityReleaseDescriptor);

        var errors=new List<string>();

        if(roiReplayBinding.ProductionSessionId==Guid.Empty)
            errors.Add("ROI replay Production session id cannot be empty.");
        if(roiReplayBinding.QualityRunId==Guid.Empty)
            errors.Add("ROI replay Quality run id cannot be empty.");
        if(qualityReleaseDescriptor.QualityRunId==Guid.Empty)
            errors.Add("Quality release replay Quality run id cannot be empty.");

        foreach(var pair in new[]
        {
            (nameof(roiReplayBinding.PcbExecutionFingerprint),roiReplayBinding.PcbExecutionFingerprint),
            (nameof(roiReplayBinding.RoiBindingFingerprint),roiReplayBinding.RoiBindingFingerprint),
            (nameof(roiReplayBinding.ReplayConvergenceFingerprint),roiReplayBinding.ReplayConvergenceFingerprint),
            (nameof(qualityReleaseDescriptor.QualitySummaryFingerprint),qualityReleaseDescriptor.QualitySummaryFingerprint),
            (nameof(qualityReleaseDescriptor.ReleaseManifestFingerprint),qualityReleaseDescriptor.ReleaseManifestFingerprint),
            (nameof(qualityReleaseDescriptor.ProjectionFingerprint),qualityReleaseDescriptor.ProjectionFingerprint),
            (nameof(qualityReleaseDescriptor.DescriptorFingerprint),qualityReleaseDescriptor.DescriptorFingerprint)
        })
        {
            if(!IsLowerHex(pair.Item2))
                errors.Add(pair.Item1+" must be 64 lowercase hexadecimal characters.");
        }

        if(string.IsNullOrWhiteSpace(roiReplayBinding.ArtifactPath))
            errors.Add("ROI replay logical artifact path cannot be blank.");

        if(roiReplayBinding.QualityRunId!=qualityReleaseDescriptor.QualityRunId)
            errors.Add("ROI replay and Quality release replay Quality identities must match.");

        if(roiReplayBinding.ReleaseReady!=qualityReleaseDescriptor.ReleaseReady)
            errors.Add("ROI replay and Quality release replay readiness must match.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateClosure(
        PcbExecutionRoiReplayBinding roiReplayBinding,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor,
        PcbExecutionRoiQualityReleaseReplayClosure closure)
    {
        ArgumentNullException.ThrowIfNull(closure);

        var errors=new List<string>(Validate(roiReplayBinding,qualityReleaseDescriptor));
        if(errors.Count>0)
            return errors;

        if(closure.ProductionSessionId!=roiReplayBinding.ProductionSessionId)
            errors.Add("Closure Production session identity must match ROI replay.");
        if(closure.QualityRunId!=roiReplayBinding.QualityRunId)
            errors.Add("Closure Quality run identity must match ROI replay.");
        if(closure.RoiBindingFingerprint!=roiReplayBinding.RoiBindingFingerprint)
            errors.Add("Closure ROI binding identity must match.");
        if(closure.ReplayConvergenceFingerprint!=roiReplayBinding.ReplayConvergenceFingerprint)
            errors.Add("Closure replay convergence identity must match.");
        if(closure.ReleaseReady!=qualityReleaseDescriptor.ReleaseReady)
            errors.Add("Closure release readiness must match Quality release replay.");
        if(closure.ArtifactPath!=roiReplayBinding.ArtifactPath)
            errors.Add("Closure artifact path must match ROI replay.");
        if(closure.QualitySummaryFingerprint!=qualityReleaseDescriptor.QualitySummaryFingerprint)
            errors.Add("Closure Quality summary identity must match.");
        if(closure.ReleaseManifestFingerprint!=qualityReleaseDescriptor.ReleaseManifestFingerprint)
            errors.Add("Closure Release manifest identity must match.");
        if(closure.QualityReleaseDescriptorFingerprint!=qualityReleaseDescriptor.DescriptorFingerprint)
            errors.Add("Closure Quality release descriptor identity must match.");
        if(!IsLowerHex(closure.ClosureFingerprint))
            errors.Add("Closure fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=Create(roiReplayBinding,qualityReleaseDescriptor);
            if(expected.ClosureFingerprint!=closure.ClosureFingerprint)
                errors.Add("Closure fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValidClosure(
        PcbExecutionRoiReplayBinding roiReplayBinding,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor,
        PcbExecutionRoiQualityReleaseReplayClosure closure)=>
        ValidateClosure(roiReplayBinding,qualityReleaseDescriptor,closure).Count==0;

    public static bool IsEquivalent(
        PcbExecutionRoiQualityReleaseReplayClosure left,
        PcbExecutionRoiQualityReleaseReplayClosure right)=>
        left.ClosureFingerprint==right.ClosureFingerprint;

    private static bool IsLowerHex(string value)=>
        value is not null &&
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
