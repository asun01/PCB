using System.Security.Cryptography;
using System.Text;
using Asun.Platform.Evidence;

namespace Asun.Platform.RenderIntegration;

public sealed record ProductionRenderEvidenceReplayDescriptor(
    long Sequence,
    string RenderFingerprint,
    EvidenceHandle EvidenceHandle,
    string DescriptorFingerprint);

public static class ProductionRenderEvidenceReplayDescriptorRuntime
{
    public static IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> Create(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames,
        IReadOnlyList<EvidenceHandle> handles)
    {
        var references=ProductionRenderEvidenceReferenceRuntime.Create(frames,handles);
        return references
            .OrderBy(reference=>reference.Sequence)
            .Select(reference=>
            {
                var canonical=string.Join(
                    "|",
                    reference.Sequence,
                    reference.RenderFingerprint,
                    reference.EvidenceHandle.Value.Length,
                    reference.EvidenceHandle.Value);
                var fingerprint=Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
                return new ProductionRenderEvidenceReplayDescriptor(
                    reference.Sequence,
                    reference.RenderFingerprint,
                    reference.EvidenceHandle,
                    fingerprint);
            })
            .ToArray();
    }

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(frames);
        ArgumentNullException.ThrowIfNull(descriptors);

        var references=descriptors
            .OrderBy(descriptor=>descriptor.Sequence)
            .Select(descriptor=>new ProductionRenderEvidenceReference(
                descriptor.Sequence,
                descriptor.RenderFingerprint,
                descriptor.EvidenceHandle))
            .ToArray();
        var errors=ProductionRenderEvidenceReferenceRuntime.Validate(frames,references);

        var count=Math.Min(references.Length,descriptors.Count);
        for(var index=0;index<count;index++)
        {
            var descriptor=descriptors.OrderBy(item=>item.Sequence).ElementAt(index);
            var reference=references[index];
            if(descriptor.DescriptorFingerprint.Length!=64 ||
               !descriptor.DescriptorFingerprint.All(Uri.IsHexDigit))
                errors.Add($"Render Evidence replay descriptor {index} fingerprint is malformed.");

            var canonical=string.Join(
                "|",
                reference.Sequence,
                reference.RenderFingerprint,
                reference.EvidenceHandle.Value.Length,
                reference.EvidenceHandle.Value);
            var expected=Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
            if(expected!=descriptor.DescriptorFingerprint)
                errors.Add($"Render Evidence replay descriptor {index} fingerprint mismatch.");
        }

        if(descriptors.Select(item=>item.Sequence).Distinct().Count()!=descriptors.Count)
            errors.Add("Render Evidence replay descriptor sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> descriptors)=>
        Validate(frames,descriptors).Count==0;

    public static bool IsEquivalent(
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> left,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        var l=left.OrderBy(item=>item.Sequence).Select(item=>item.DescriptorFingerprint);
        var r=right.OrderBy(item=>item.Sequence).Select(item=>item.DescriptorFingerprint);
        return l.SequenceEqual(r);
    }
}
