using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbExecutionIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionExecutionProvenanceAuditClosure(
    Guid ProductionSessionId,
    string AssemblyFingerprint,
    string PcbExecutionBindingFingerprint,
    string UnifiedReplayClosureFingerprint,
    long Sequence,
    string ProductionInputFingerprint,
    Guid QualityRunId,
    string ReleaseManifestFingerprint,
    string ProvenanceDescriptorFingerprint,
    string ReplayDescriptorFingerprint,
    string Fingerprint);

public static class ProductionExecutionProvenanceAuditClosureRuntime
{
    public static ProductionExecutionProvenanceAuditClosure Create(
        PcbExecutionBoardBinding boardBinding,
        UnifiedReplayClosure unifiedClosure,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor provenanceDescriptor,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor)
    {
        ArgumentNullException.ThrowIfNull(boardBinding);
        ArgumentNullException.ThrowIfNull(unifiedClosure);
        ArgumentNullException.ThrowIfNull(provenanceDescriptor);
        ArgumentNullException.ThrowIfNull(replayDescriptor);

        var errors=ValidateInputs(
            boardBinding,
            unifiedClosure,
            provenanceDescriptor,
            replayDescriptor);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(boardBinding));

        var fingerprint=CreateFingerprint(
            boardBinding.ProductionSessionId,
            boardBinding.AssemblyFingerprint,
            boardBinding.BindingFingerprint,
            unifiedClosure.Fingerprint,
            provenanceDescriptor.Sequence,
            provenanceDescriptor.ProductionInputFingerprint,
            replayDescriptor.QualityRunId,
            replayDescriptor.ReleaseManifestFingerprint,
            provenanceDescriptor.ProvenanceFingerprint,
            replayDescriptor.DescriptorFingerprint);

        return new ProductionExecutionProvenanceAuditClosure(
            boardBinding.ProductionSessionId,
            boardBinding.AssemblyFingerprint,
            boardBinding.BindingFingerprint,
            unifiedClosure.Fingerprint,
            provenanceDescriptor.Sequence,
            provenanceDescriptor.ProductionInputFingerprint,
            replayDescriptor.QualityRunId,
            replayDescriptor.ReleaseManifestFingerprint,
            provenanceDescriptor.ProvenanceFingerprint,
            replayDescriptor.DescriptorFingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        PcbExecutionBoardBinding boardBinding,
        UnifiedReplayClosure unifiedClosure,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor provenanceDescriptor,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor,
        ProductionExecutionProvenanceAuditClosure closure)
    {
        ArgumentNullException.ThrowIfNull(boardBinding);
        ArgumentNullException.ThrowIfNull(unifiedClosure);
        ArgumentNullException.ThrowIfNull(provenanceDescriptor);
        ArgumentNullException.ThrowIfNull(replayDescriptor);
        ArgumentNullException.ThrowIfNull(closure);

        var errors=ValidateInputs(
            boardBinding,
            unifiedClosure,
            provenanceDescriptor,
            replayDescriptor);

        if(closure.ProductionSessionId!=boardBinding.ProductionSessionId)
            errors.Add("Audit closure Production session identity must match PCB execution binding.");
        if(closure.AssemblyFingerprint!=boardBinding.AssemblyFingerprint)
            errors.Add("Audit closure assembly identity must match PCB execution binding.");
        if(closure.PcbExecutionBindingFingerprint!=boardBinding.BindingFingerprint)
            errors.Add("Audit closure PCB execution binding identity must match.");
        if(closure.UnifiedReplayClosureFingerprint!=unifiedClosure.Fingerprint)
            errors.Add("Audit closure Unified Replay identity must match.");
        if(closure.Sequence!=provenanceDescriptor.Sequence)
            errors.Add("Audit closure sequence must match provenance.");
        if(closure.ProductionInputFingerprint!=provenanceDescriptor.ProductionInputFingerprint)
            errors.Add("Audit closure Production input identity must match provenance.");
        if(closure.QualityRunId!=replayDescriptor.QualityRunId)
            errors.Add("Audit closure Quality run identity must match Replay descriptor.");
        if(closure.ReleaseManifestFingerprint!=replayDescriptor.ReleaseManifestFingerprint)
            errors.Add("Audit closure Release manifest identity must match Replay descriptor.");
        if(closure.ProvenanceDescriptorFingerprint!=provenanceDescriptor.ProvenanceFingerprint)
            errors.Add("Audit closure provenance identity must match.");
        if(closure.ReplayDescriptorFingerprint!=replayDescriptor.DescriptorFingerprint)
            errors.Add("Audit closure Replay descriptor identity must match.");
        if(closure.Fingerprint.Length!=64 || !IsLowerHex(closure.Fingerprint))
            errors.Add("Audit closure fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                boardBinding.ProductionSessionId,
                boardBinding.AssemblyFingerprint,
                boardBinding.BindingFingerprint,
                unifiedClosure.Fingerprint,
                provenanceDescriptor.Sequence,
                provenanceDescriptor.ProductionInputFingerprint,
                replayDescriptor.QualityRunId,
                replayDescriptor.ReleaseManifestFingerprint,
                provenanceDescriptor.ProvenanceFingerprint,
                replayDescriptor.DescriptorFingerprint);

            if(expected!=closure.Fingerprint)
                errors.Add("Audit closure fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        PcbExecutionBoardBinding boardBinding,
        UnifiedReplayClosure unifiedClosure,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor provenanceDescriptor,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor,
        ProductionExecutionProvenanceAuditClosure closure)=>
        Validate(
            boardBinding,
            unifiedClosure,
            provenanceDescriptor,
            replayDescriptor,
            closure).Count==0;

    internal static string CreateFingerprint(
        Guid productionSessionId,
        string assemblyFingerprint,
        string pcbExecutionBindingFingerprint,
        string unifiedReplayClosureFingerprint,
        long sequence,
        string productionInputFingerprint,
        Guid qualityRunId,
        string releaseManifestFingerprint,
        string provenanceDescriptorFingerprint,
        string replayDescriptorFingerprint)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            assemblyFingerprint,
            pcbExecutionBindingFingerprint,
            unifiedReplayClosureFingerprint,
            sequence,
            productionInputFingerprint,
            qualityRunId,
            releaseManifestFingerprint,
            provenanceDescriptorFingerprint,
            replayDescriptorFingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static List<string> ValidateInputs(
        PcbExecutionBoardBinding boardBinding,
        UnifiedReplayClosure unifiedClosure,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor provenanceDescriptor,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor)
    {
        var errors=new List<string>();

        if(boardBinding.ProductionSessionId==Guid.Empty)
            errors.Add("PCB execution binding Production session identity is invalid.");
        if(!IsLowerHex(boardBinding.AssemblyFingerprint))
            errors.Add("PCB execution binding assembly fingerprint is malformed.");
        if(!IsLowerHex(boardBinding.BindingFingerprint))
            errors.Add("PCB execution binding fingerprint is malformed.");
        if(unifiedClosure.SessionId!=boardBinding.ProductionSessionId)
            errors.Add("Unified Replay session identity must match PCB execution binding.");
        if(!IsLowerHex(unifiedClosure.Fingerprint))
            errors.Add("Unified Replay closure fingerprint is malformed.");
        if(replayDescriptor.ProductionSessionId!=boardBinding.ProductionSessionId)
            errors.Add("Replay descriptor Production session identity must match PCB execution binding.");
        if(replayDescriptor.Sequence!=provenanceDescriptor.Sequence)
            errors.Add("Replay descriptor sequence must match provenance descriptor.");
        if(replayDescriptor.ProductionInputFingerprint!=provenanceDescriptor.ProductionInputFingerprint)
            errors.Add("Replay descriptor Production input identity must match provenance descriptor.");
        if(provenanceDescriptor.ReleaseReplayDescriptorFingerprint!=replayDescriptor.DescriptorFingerprint)
            errors.Add("Provenance descriptor must reference the same Replay descriptor.");
        if(!IsLowerHex(provenanceDescriptor.ProductionInputFingerprint))
            errors.Add("Provenance Production input fingerprint is malformed.");
        if(!IsLowerHex(provenanceDescriptor.ProvenanceFingerprint))
            errors.Add("Provenance descriptor fingerprint is malformed.");
        if(!IsLowerHex(replayDescriptor.DescriptorFingerprint))
            errors.Add("Replay descriptor fingerprint is malformed.");
        if(!IsLowerHex(replayDescriptor.ReleaseManifestFingerprint))
            errors.Add("Replay descriptor Release manifest fingerprint is malformed.");
        if(replayDescriptor.QualityRunId==Guid.Empty)
            errors.Add("Replay descriptor Quality run identity is invalid.");

        return errors;
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
