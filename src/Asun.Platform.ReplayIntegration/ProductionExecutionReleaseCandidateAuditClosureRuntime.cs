using System.Security.Cryptography;
using System.Text;
using Asun.Release.Core;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionExecutionReleaseCandidateAuditClosure(
    Guid ProductionSessionId,
    string AuditClosureFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string ArtifactPath,
    string Fingerprint);

public static class ProductionExecutionReleaseCandidateAuditClosureRuntime
{
    public static ProductionExecutionReleaseCandidateAuditClosure Create(
        ProductionExecutionProvenanceAuditClosure auditClosure,
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport report)
    {
        ArgumentNullException.ThrowIfNull(auditClosure);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);

        var errors=ValidateInputs(auditClosure,manifest,definition,report);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(manifest));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var artifactPath=manifest.Artifacts[0].Path;
        var fingerprint=CreateFingerprint(
            report.SessionId,
            auditClosure.Fingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            artifactPath);

        return new ProductionExecutionReleaseCandidateAuditClosure(
            report.SessionId,
            auditClosure.Fingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            artifactPath,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionExecutionProvenanceAuditClosure auditClosure,
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport report,
        ProductionExecutionReleaseCandidateAuditClosure closure)
    {
        ArgumentNullException.ThrowIfNull(auditClosure);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(closure);

        var errors=ValidateInputs(auditClosure,manifest,definition,report);
        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);

        if(closure.ProductionSessionId!=report.SessionId)
            errors.Add("Release candidate audit Production session identity must match report.");
        if(closure.ProductionSessionId!=auditClosure.ProductionSessionId)
            errors.Add("Release candidate audit Production session identity must match audit closure.");
        if(closure.AuditClosureFingerprint!=auditClosure.Fingerprint)
            errors.Add("Release candidate audit closure identity must match.");
        if(closure.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release candidate audit manifest identity must match.");
        if(closure.ReleaseReady!=readiness.Ready)
            errors.Add("Release candidate audit readiness must match canonical Release readiness.");
        if(closure.ArtifactPath!=manifest.Artifacts[0].Path)
            errors.Add("Release candidate audit artifact path must match logical Release artifact.");
        if(closure.Fingerprint.Length!=64 || !IsLowerHex(closure.Fingerprint))
            errors.Add("Release candidate audit fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                report.SessionId,
                auditClosure.Fingerprint,
                manifest.Fingerprint,
                readiness.Ready,
                manifest.Artifacts[0].Path);
            if(expected!=closure.Fingerprint)
                errors.Add("Release candidate audit fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionExecutionProvenanceAuditClosure auditClosure,
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport report,
        ProductionExecutionReleaseCandidateAuditClosure closure)=>
        Validate(auditClosure,manifest,definition,report,closure).Count==0;

    internal static string CreateFingerprint(
        Guid productionSessionId,
        string auditClosureFingerprint,
        string releaseManifestFingerprint,
        bool releaseReady,
        string artifactPath)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            auditClosureFingerprint,
            releaseManifestFingerprint,
            releaseReady,
            artifactPath.Length,
            artifactPath);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static List<string> ValidateInputs(
        ProductionExecutionProvenanceAuditClosure auditClosure,
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport report)
    {
        var errors=new List<string>();

        if(auditClosure.ProductionSessionId!=report.SessionId)
            errors.Add("Audit closure Production session must match Production report.");
        if(!IsLowerHex(auditClosure.Fingerprint))
            errors.Add("Audit closure fingerprint is malformed.");
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        errors.AddRange(
            ProductionSessionValidationRuntime.Validate(
                definition,
                report));

        if(errors.Count>0)
            return errors;

        if(!ProductionReleaseCandidateValidationRuntime.IsValid(
            manifest,
            definition,
            report))
            errors.Add("Production release candidate is invalid.");

        if(manifest.Artifacts.Count!=1)
            errors.Add("Production release candidate must expose exactly one logical artifact.");

        if(manifest.Artifacts.Count==1 &&
           manifest.Fingerprint!=auditClosure.ReleaseManifestFingerprint)
            errors.Add("Release candidate manifest fingerprint must match audit closure Release identity.");

        if(string.IsNullOrWhiteSpace(manifest.Artifacts[0].Path))
            errors.Add("Release candidate artifact path is required.");

        return errors;
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
