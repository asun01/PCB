using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record UnifiedReplayClosure(
    Guid SessionId,
    string ProgramFingerprint,
    string PipelineIdentityFingerprint,
    string SimulationDescriptorFingerprint,
    int RenderEvidenceDescriptorCount,
    string RenderEvidenceDescriptorFingerprint,
    string QualityReleaseDescriptorFingerprint,
    string BundleReleaseBindingFingerprint,
    string Fingerprint);

public static class UnifiedReplayClosureRuntime
{
    public static UnifiedReplayClosure Create(
        ProductionPipelineExecutionIdentity pipelineIdentity,
        ProductionSimulationReplayDescriptor simulationDescriptor,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> renderDescriptors,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor,
        ProductionQualityEvidenceReleaseReplayBinding releaseBinding)
    {
        ArgumentNullException.ThrowIfNull(pipelineIdentity);
        ArgumentNullException.ThrowIfNull(simulationDescriptor);
        ArgumentNullException.ThrowIfNull(renderDescriptors);
        ArgumentNullException.ThrowIfNull(qualityReleaseDescriptor);
        ArgumentNullException.ThrowIfNull(releaseBinding);

        var errors=ValidateInputs(
            pipelineIdentity,
            simulationDescriptor,
            renderDescriptors,
            qualityReleaseDescriptor,
            releaseBinding);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(renderDescriptors));

        var renderFingerprint=CreateRenderDescriptorFingerprint(renderDescriptors);
        var fingerprint=CreateFingerprint(
            pipelineIdentity.SessionId,
            pipelineIdentity.ProgramFingerprint,
            pipelineIdentity.Fingerprint,
            simulationDescriptor.DescriptorFingerprint,
            renderDescriptors.Count,
            renderFingerprint,
            qualityReleaseDescriptor.DescriptorFingerprint,
            releaseBinding.Fingerprint);

        return new UnifiedReplayClosure(
            pipelineIdentity.SessionId,
            pipelineIdentity.ProgramFingerprint,
            pipelineIdentity.Fingerprint,
            simulationDescriptor.DescriptorFingerprint,
            renderDescriptors.Count,
            renderFingerprint,
            qualityReleaseDescriptor.DescriptorFingerprint,
            releaseBinding.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionPipelineExecutionIdentity pipelineIdentity,
        ProductionSimulationReplayDescriptor simulationDescriptor,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> renderDescriptors,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor,
        ProductionQualityEvidenceReleaseReplayBinding releaseBinding,
        UnifiedReplayClosure closure)
    {
        ArgumentNullException.ThrowIfNull(closure);

        var errors=ValidateInputs(
            pipelineIdentity,
            simulationDescriptor,
            renderDescriptors,
            qualityReleaseDescriptor,
            releaseBinding);

        if(closure.SessionId!=pipelineIdentity.SessionId)
            errors.Add("Unified replay closure session identity must match.");
        if(closure.ProgramFingerprint!=pipelineIdentity.ProgramFingerprint)
            errors.Add("Unified replay closure Program fingerprint must match.");
        if(closure.PipelineIdentityFingerprint!=pipelineIdentity.Fingerprint)
            errors.Add("Unified replay closure pipeline identity must match.");
        if(closure.SimulationDescriptorFingerprint!=simulationDescriptor.DescriptorFingerprint)
            errors.Add("Unified replay closure simulation identity must match.");
        if(closure.RenderEvidenceDescriptorCount!=renderDescriptors.Count)
            errors.Add("Unified replay closure render descriptor count must match.");
        if(closure.RenderEvidenceDescriptorFingerprint!=CreateRenderDescriptorFingerprint(renderDescriptors))
            errors.Add("Unified replay closure render descriptor fingerprint must match.");
        if(closure.QualityReleaseDescriptorFingerprint!=qualityReleaseDescriptor.DescriptorFingerprint)
            errors.Add("Unified replay closure Quality/Release identity must match.");
        if(closure.BundleReleaseBindingFingerprint!=releaseBinding.Fingerprint)
            errors.Add("Unified replay closure Release binding identity must match.");
        if(closure.Fingerprint.Length!=64 ||
           !closure.Fingerprint.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
            errors.Add("Unified replay closure fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                pipelineIdentity.SessionId,
                pipelineIdentity.ProgramFingerprint,
                pipelineIdentity.Fingerprint,
                simulationDescriptor.DescriptorFingerprint,
                renderDescriptors.Count,
                CreateRenderDescriptorFingerprint(renderDescriptors),
                qualityReleaseDescriptor.DescriptorFingerprint,
                releaseBinding.Fingerprint);
            if(expected!=closure.Fingerprint)
                errors.Add("Unified replay closure fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionPipelineExecutionIdentity pipelineIdentity,
        ProductionSimulationReplayDescriptor simulationDescriptor,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> renderDescriptors,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor,
        ProductionQualityEvidenceReleaseReplayBinding releaseBinding,
        UnifiedReplayClosure closure)=>
        Validate(
            pipelineIdentity,
            simulationDescriptor,
            renderDescriptors,
            qualityReleaseDescriptor,
            releaseBinding,
            closure).Count==0;

    internal static string CreateRenderDescriptorFingerprint(
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> descriptors)
    {
        var canonical=string.Join(
            "|",
            descriptors
                .OrderBy(item=>item.Sequence)
                .Select(item=>$"{item.Sequence}:{item.DescriptorFingerprint}"));
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static List<string> ValidateInputs(
        ProductionPipelineExecutionIdentity pipelineIdentity,
        ProductionSimulationReplayDescriptor simulationDescriptor,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> renderDescriptors,
        QualityReleaseReplayDescriptor qualityReleaseDescriptor,
        ProductionQualityEvidenceReleaseReplayBinding releaseBinding)
    {
        ArgumentNullException.ThrowIfNull(pipelineIdentity);
        ArgumentNullException.ThrowIfNull(simulationDescriptor);
        ArgumentNullException.ThrowIfNull(renderDescriptors);
        ArgumentNullException.ThrowIfNull(qualityReleaseDescriptor);
        ArgumentNullException.ThrowIfNull(releaseBinding);

        var errors=new List<string>();

        if(pipelineIdentity.SessionId==Guid.Empty)
            errors.Add("Pipeline identity session id is invalid.");
        if(simulationDescriptor.ProductionSessionId!=pipelineIdentity.SessionId)
            errors.Add("Simulation descriptor session identity must match pipeline identity.");
        if(releaseBinding.ProductionSessionId!=pipelineIdentity.SessionId)
            errors.Add("Release binding session identity must match pipeline identity.");
        if(simulationDescriptor.ProductionFingerprint!=pipelineIdentity.ProductionFingerprint)
            errors.Add("Simulation descriptor production fingerprint must match pipeline identity.");
        if(releaseBinding.ReplayBundleFingerprint.Length!=64 ||
           !releaseBinding.ReplayBundleFingerprint.All(Uri.IsHexDigit))
            errors.Add("Release binding replay bundle identity is malformed.");
        if(releaseBinding.Fingerprint.Length!=64)
            errors.Add("Release binding fingerprint is malformed.");
        if(qualityReleaseDescriptor.QualityRunId!=releaseBinding.QualityRunId)
            errors.Add("Quality/Release descriptor run identity must match Release binding.");
        if(qualityReleaseDescriptor.ReleaseManifestFingerprint!=releaseBinding.ReleaseManifestFingerprint)
            errors.Add("Quality/Release descriptor manifest identity must match Release binding.");
        if(qualityReleaseDescriptor.DescriptorFingerprint.Length!=64)
            errors.Add("Quality/Release descriptor fingerprint is malformed.");
        if(simulationDescriptor.DescriptorFingerprint.Length!=64)
            errors.Add("Simulation descriptor fingerprint is malformed.");
        if(pipelineIdentity.Fingerprint.Length!=64)
            errors.Add("Pipeline identity fingerprint is malformed.");

        if(renderDescriptors.Count==0)
            errors.Add("Unified replay closure requires at least one Render/Evidence descriptor.");

        if(renderDescriptors.Any(item=>
            item.Sequence<=0 ||
            item.DescriptorFingerprint.Length!=64 ||
            !item.EvidenceHandle.IsValid ||
            string.IsNullOrWhiteSpace(item.RenderFingerprint)))
            errors.Add("Render/Evidence descriptor collection contains an invalid identity.");

        if(renderDescriptors.Select(item=>item.Sequence).Distinct().Count()!=renderDescriptors.Count)
            errors.Add("Render/Evidence descriptor sequences must be unique.");

        if(renderDescriptors.Select(item=>item.DescriptorFingerprint).Distinct().Count()!=renderDescriptors.Count)
            errors.Add("Render/Evidence descriptor fingerprints must be unique.");

        return errors;
    }

    private static string CreateFingerprint(
        Guid sessionId,
        string programFingerprint,
        string pipelineIdentityFingerprint,
        string simulationDescriptorFingerprint,
        int renderDescriptorCount,
        string renderDescriptorFingerprint,
        string qualityReleaseDescriptorFingerprint,
        string bundleReleaseBindingFingerprint)
    {
        var canonical=string.Join(
            "|",
            sessionId,
            programFingerprint,
            pipelineIdentityFingerprint,
            simulationDescriptorFingerprint,
            renderDescriptorCount,
            renderDescriptorFingerprint,
            qualityReleaseDescriptorFingerprint,
            bundleReleaseBindingFingerprint);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}
