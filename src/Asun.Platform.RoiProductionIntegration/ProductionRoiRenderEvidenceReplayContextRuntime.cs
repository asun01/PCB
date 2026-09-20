using System.Security.Cryptography;
using System.Text;
using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.RoiProductionIntegration;

public sealed record ProductionRoiRenderEvidenceReplayContext(
    Guid ProductionSessionId,
    string RoiContextBindingFingerprint,
    int EvidenceCount,
    string EvidenceFingerprint,
    string BindingFingerprint);

public static class ProductionRoiRenderEvidenceReplayContextRuntime
{
    public static ProductionRoiRenderEvidenceReplayContext Create(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> evidenceDescriptors)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(renderFrames);
        ArgumentNullException.ThrowIfNull(evidenceDescriptors);

        var errors=Validate(productionReport,roiContext,renderFrames,evidenceDescriptors);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var evidenceFingerprint=CreateEvidenceFingerprint(evidenceDescriptors);
        var canonical=string.Join("|",
            productionReport.SessionId,
            roiContext.BindingFingerprint,
            evidenceDescriptors.Count,
            evidenceFingerprint);

        return new ProductionRoiRenderEvidenceReplayContext(
            productionReport.SessionId,
            roiContext.BindingFingerprint,
            evidenceDescriptors.Count,
            evidenceFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> evidenceDescriptors)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(renderFrames);
        ArgumentNullException.ThrowIfNull(evidenceDescriptors);

        var errors=new List<string>();
        errors.AddRange(
            ProductionRoiRenderReplayContextRuntime.Validate(
                productionReport,
                roiContext,
                renderFrames));

        errors.AddRange(
            ProductionRenderEvidenceReplayDescriptorRuntime.Validate(
                renderFrames,
                evidenceDescriptors));

        if(evidenceDescriptors.Count!=renderFrames.Count)
            errors.Add("Evidence descriptor count must match render replay frame count.");

        if(evidenceDescriptors.Select(item=>item.EvidenceHandle.Value)
            .Distinct(StringComparer.Ordinal).Count()!=evidenceDescriptors.Count)
            errors.Add("Evidence handles must be unique in the replay context.");

        foreach(var descriptor in evidenceDescriptors)
        {
            if(string.IsNullOrWhiteSpace(descriptor.EvidenceHandle.Value))
                errors.Add("Evidence handle values cannot be blank.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> evidenceDescriptors)=>
        Validate(productionReport,roiContext,renderFrames,evidenceDescriptors).Count==0;

    public static bool IsEquivalent(
        ProductionRoiRenderEvidenceReplayContext left,
        ProductionRoiRenderEvidenceReplayContext right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static string CreateEvidenceFingerprint(
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> descriptors)
    {
        var canonical=string.Join(
            "
",
            descriptors.OrderBy(item=>item.Sequence)
                .Select(item=>$"{item.Sequence}|{item.RenderFingerprint}|{item.EvidenceHandle.Value}|{item.DescriptorFingerprint}"));
        return Hash(canonical);
    }

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
