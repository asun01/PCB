using System.Security.Cryptography;
using System.Text;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.PcbExecutionIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record PcbMeasurementReplayComponentBinding(
    Guid ProductionSessionId,
    Guid QualityRunId,
    int MeasurementCount,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    IReadOnlyList<string> ComponentIds,
    IReadOnlyList<string> ReplayDescriptorFingerprints,
    string BindingFingerprint);

public static class PcbMeasurementReplayComponentBindingRuntime
{
    public static PcbMeasurementReplayComponentBinding Create(
        PcbExecutionMeasurementComponentBinding executionBinding,
        IReadOnlyList<ProductionMeasurementPcbBinding> measurementBindings,
        IReadOnlyList<ProductionMeasurementQualityEvidenceReleaseReplayDescriptor> replayDescriptors)
    {
        ArgumentNullException.ThrowIfNull(executionBinding);
        ArgumentNullException.ThrowIfNull(measurementBindings);
        ArgumentNullException.ThrowIfNull(replayDescriptors);

        var errors=Validate(executionBinding,measurementBindings,replayDescriptors);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var orderedBindings=measurementBindings.OrderBy(item=>item.Sequence).ToArray();
        var orderedDescriptors=replayDescriptors.OrderBy(item=>item.Sequence).ToArray();

        var manifest=orderedDescriptors[0].ReleaseManifestFingerprint;
        var ready=orderedDescriptors[0].ReleaseReady;
        var descriptorFingerprints=orderedDescriptors.Select(item=>item.DescriptorFingerprint).ToArray();
        var componentIds=orderedBindings.Select(item=>item.ComponentId.Value).ToArray();

        var canonical=string.Join("|",
            executionBinding.ProductionSessionId,
            orderedDescriptors[0].QualityRunId,
            executionBinding.MeasurementBindingsFingerprint,
            manifest,
            ready,
            string.Join(",",componentIds),
            string.Join(",",descriptorFingerprints));

        return new PcbMeasurementReplayComponentBinding(
            executionBinding.ProductionSessionId,
            orderedDescriptors[0].QualityRunId,
            orderedBindings.Length,
            manifest,
            ready,
            componentIds,
            descriptorFingerprints,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        PcbExecutionMeasurementComponentBinding executionBinding,
        IReadOnlyList<ProductionMeasurementPcbBinding> measurementBindings,
        IReadOnlyList<ProductionMeasurementQualityEvidenceReleaseReplayDescriptor> replayDescriptors)
    {
        ArgumentNullException.ThrowIfNull(executionBinding);
        ArgumentNullException.ThrowIfNull(measurementBindings);
        ArgumentNullException.ThrowIfNull(replayDescriptors);

        var errors=new List<string>();

        if(executionBinding.MeasurementFactCount!=measurementBindings.Count)
            errors.Add("Execution measurement count must match measurement binding count.");
        if(measurementBindings.Count!=replayDescriptors.Count)
            errors.Add("Measurement binding count must match replay descriptor count.");

        var bindings=measurementBindings.OrderBy(item=>item.Sequence).ToArray();
        var descriptors=replayDescriptors.OrderBy(item=>item.Sequence).ToArray();

        if(bindings.Select(item=>item.Sequence).Distinct().Count()!=bindings.Length)
            errors.Add("Measurement binding sequences must be unique.");
        if(descriptors.Select(item=>item.Sequence).Distinct().Count()!=descriptors.Length)
            errors.Add("Replay descriptor sequences must be unique.");

        if(descriptors.Length>0)
        {
            var manifest=descriptors[0].ReleaseManifestFingerprint;
            var ready=descriptors[0].ReleaseReady;
            var qualityRunId=descriptors[0].QualityRunId;
            if(qualityRunId==Guid.Empty)
                errors.Add("Replay descriptor Quality run identity cannot be empty.");

            for(var index=0;index<descriptors.Length;index++)
            {
                var descriptor=descriptors[index];
                if(descriptor.Sequence!=bindings[index].Sequence)
                    errors.Add($"Replay descriptor {index} sequence does not match the measurement binding.");
                if(descriptor.ProductionInputFingerprint!=bindings[index].ProductionInputFingerprint)
                    errors.Add($"Replay descriptor {index} Production input fingerprint does not match the measurement binding.");
                if(descriptor.ComponentId!=bindings[index].ComponentId.Value)
                    errors.Add($"Replay descriptor {index} component identity does not match the measurement binding.");
                if(descriptor.QualityResultId==Guid.Empty)
                    errors.Add($"Replay descriptor {index} Quality result identity cannot be empty.");
                if(!IsLowerHex(descriptor.EvidenceFingerprint))
                    errors.Add($"Replay descriptor {index} Evidence fingerprint is malformed.");
                if(!IsLowerHex(descriptor.ReplayBundleFingerprint))
                    errors.Add($"Replay descriptor {index} replay bundle fingerprint is malformed.");
                if(!IsLowerHex(descriptor.ReleaseManifestFingerprint) ||
                   descriptor.ReleaseManifestFingerprint!=manifest)
                    errors.Add($"Replay descriptor {index} Release manifest identity is inconsistent.");
                if(descriptor.ReleaseReady!=ready)
                    errors.Add($"Replay descriptor {index} Release readiness is inconsistent.");
                if(!IsLowerHex(descriptor.ReleaseReplayBindingFingerprint))
                    errors.Add($"Replay descriptor {index} Release replay binding fingerprint is malformed.");
                if(!IsLowerHex(descriptor.DescriptorFingerprint))
                    errors.Add($"Replay descriptor {index} descriptor fingerprint is malformed.");
                if(descriptor.ProductionSessionId==Guid.Empty ||
                   descriptor.ProductionSessionId!=executionBinding.ProductionSessionId)
                    errors.Add($"Replay descriptor {index} Production session identity must match the execution binding.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        PcbExecutionMeasurementComponentBinding executionBinding,
        IReadOnlyList<ProductionMeasurementPcbBinding> measurementBindings,
        IReadOnlyList<ProductionMeasurementQualityEvidenceReleaseReplayDescriptor> replayDescriptors)=>
        Validate(executionBinding,measurementBindings,replayDescriptors).Count==0;

    public static bool IsEquivalent(
        PcbMeasurementReplayComponentBinding left,
        PcbMeasurementReplayComponentBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
