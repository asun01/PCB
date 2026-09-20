using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.ReplayIntegration;

public sealed record PcbExecutionRoiQualityReleaseAuditReplayClosure(
    Guid ProductionSessionId,
    Guid QualityRunId,
    string RoiBindingFingerprint,
    string ReleaseManifestFingerprint,
    string AuditReplayBindingFingerprint,
    string ReplayDescriptorFingerprint,
    string ClosureFingerprint);

public static class PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime
{
    public static PcbExecutionRoiQualityReleaseAuditReplayClosure Create(
        PcbExecutionRoiQualityReleaseReplayClosure roiQualityReleaseClosure,
        ProductionMeasurementQualityEvidenceReleaseAuditReplayContext auditContext)
    {
        ArgumentNullException.ThrowIfNull(roiQualityReleaseClosure);
        ArgumentNullException.ThrowIfNull(auditContext);

        var errors=Validate(roiQualityReleaseClosure,auditContext);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            roiQualityReleaseClosure.ProductionSessionId,
            roiQualityReleaseClosure.QualityRunId,
            roiQualityReleaseClosure.RoiBindingFingerprint,
            roiQualityReleaseClosure.ReleaseManifestFingerprint,
            roiQualityReleaseClosure.QualityReleaseDescriptorFingerprint,
            auditContext.ProductionSessionId,
            auditContext.QualityRunId,
            auditContext.ReleaseManifestFingerprint,
            auditContext.AuditReplayBindingFingerprint,
            auditContext.ReplayDescriptorFingerprint);

        return new PcbExecutionRoiQualityReleaseAuditReplayClosure(
            roiQualityReleaseClosure.ProductionSessionId,
            roiQualityReleaseClosure.QualityRunId,
            roiQualityReleaseClosure.RoiBindingFingerprint,
            roiQualityReleaseClosure.ReleaseManifestFingerprint,
            auditContext.AuditReplayBindingFingerprint,
            auditContext.ReplayDescriptorFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        PcbExecutionRoiQualityReleaseReplayClosure roiQualityReleaseClosure,
        ProductionMeasurementQualityEvidenceReleaseAuditReplayContext auditContext)
    {
        ArgumentNullException.ThrowIfNull(roiQualityReleaseClosure);
        ArgumentNullException.ThrowIfNull(auditContext);

        var errors=new List<string>();

        if(roiQualityReleaseClosure.ProductionSessionId==Guid.Empty)
            errors.Add("ROI quality-release Production session id cannot be empty.");
        if(roiQualityReleaseClosure.QualityRunId==Guid.Empty)
            errors.Add("ROI quality-release Quality run id cannot be empty.");
        if(auditContext.ProductionSessionId==Guid.Empty)
            errors.Add("Audit replay Production session id cannot be empty.");
        if(auditContext.QualityRunId==Guid.Empty)
            errors.Add("Audit replay Quality run id cannot be empty.");

        foreach(var pair in new[]
        {
            (nameof(roiQualityReleaseClosure.RoiBindingFingerprint),roiQualityReleaseClosure.RoiBindingFingerprint),
            (nameof(roiQualityReleaseClosure.ReleaseManifestFingerprint),roiQualityReleaseClosure.ReleaseManifestFingerprint),
            (nameof(roiQualityReleaseClosure.QualityReleaseDescriptorFingerprint),roiQualityReleaseClosure.QualityReleaseDescriptorFingerprint),
            (nameof(auditContext.ReleaseManifestFingerprint),auditContext.ReleaseManifestFingerprint),
            (nameof(auditContext.AuditReplayBindingFingerprint),auditContext.AuditReplayBindingFingerprint),
            (nameof(auditContext.ReplayDescriptorFingerprint),auditContext.ReplayDescriptorFingerprint)
        })
        {
            if(!IsLowerHex(pair.Item2))
                errors.Add(pair.Item1+" must be 64 lowercase hexadecimal characters.");
        }

        if(roiQualityReleaseClosure.ProductionSessionId!=auditContext.ProductionSessionId)
            errors.Add("ROI quality-release and audit replay Production session identities must match.");
        if(roiQualityReleaseClosure.QualityRunId!=auditContext.QualityRunId)
            errors.Add("ROI quality-release and audit replay Quality identities must match.");
        if(roiQualityReleaseClosure.ReleaseManifestFingerprint!=auditContext.ReleaseManifestFingerprint)
            errors.Add("ROI quality-release and audit replay Release manifest identities must match.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateClosure(
        PcbExecutionRoiQualityReleaseReplayClosure roiQualityReleaseClosure,
        ProductionMeasurementQualityEvidenceReleaseAuditReplayContext auditContext,
        PcbExecutionRoiQualityReleaseAuditReplayClosure closure)
    {
        var errors=new List<string>(Validate(roiQualityReleaseClosure,auditContext));
        if(errors.Count>0)
            return errors;

        if(closure.ProductionSessionId!=roiQualityReleaseClosure.ProductionSessionId)
            errors.Add("Audit closure Production session identity must match.");
        if(closure.QualityRunId!=roiQualityReleaseClosure.QualityRunId)
            errors.Add("Audit closure Quality run identity must match.");
        if(closure.RoiBindingFingerprint!=roiQualityReleaseClosure.RoiBindingFingerprint)
            errors.Add("Audit closure ROI binding identity must match.");
        if(closure.ReleaseManifestFingerprint!=roiQualityReleaseClosure.ReleaseManifestFingerprint)
            errors.Add("Audit closure Release manifest identity must match.");
        if(closure.AuditReplayBindingFingerprint!=auditContext.AuditReplayBindingFingerprint)
            errors.Add("Audit closure audit replay binding identity must match.");
        if(closure.ReplayDescriptorFingerprint!=auditContext.ReplayDescriptorFingerprint)
            errors.Add("Audit closure replay descriptor identity must match.");
        if(!IsLowerHex(closure.ClosureFingerprint))
            errors.Add("Audit closure fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=Create(roiQualityReleaseClosure,auditContext);
            if(expected.ClosureFingerprint!=closure.ClosureFingerprint)
                errors.Add("Audit closure fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValidClosure(
        PcbExecutionRoiQualityReleaseReplayClosure roiQualityReleaseClosure,
        ProductionMeasurementQualityEvidenceReleaseAuditReplayContext auditContext,
        PcbExecutionRoiQualityReleaseAuditReplayClosure closure)=>
        ValidateClosure(roiQualityReleaseClosure,auditContext,closure).Count==0;

    public static bool IsEquivalent(
        PcbExecutionRoiQualityReleaseAuditReplayClosure left,
        PcbExecutionRoiQualityReleaseAuditReplayClosure right)=>
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
