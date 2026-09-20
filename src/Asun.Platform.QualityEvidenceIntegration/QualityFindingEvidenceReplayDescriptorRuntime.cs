using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;

namespace Asun.Platform.QualityEvidenceIntegration;

public sealed record QualityFindingEvidenceReplayDescriptor(
    QualityFindingId FindingId,
    IReadOnlyList<EvidenceHandle> EvidenceHandles,
    string ResolutionFingerprint,
    string DescriptorFingerprint);

public static class QualityFindingEvidenceReplayDescriptorRuntime
{
    public static IReadOnlyList<QualityFindingEvidenceReplayDescriptor> Create(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions)
    {
        if(!QualityFindingEvidenceResolutionRuntime.IsValid(run,bindings,resolutions))
            throw new ArgumentException("Quality finding evidence resolution is invalid.",nameof(resolutions));

        return resolutions
            .OrderBy(item=>item.FindingId.Value,StringComparer.Ordinal)
            .Select(item =>
            {
                var canonical=string.Join("|",
                    item.FindingId.Value,
                    string.Join(",",item.EvidenceHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal).Select(handle=>handle.Value.Length+":"+handle.Value)));
                var resolutionFingerprint=Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
                var descriptorFingerprint=Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(
                        string.Join("|",item.FindingId.Value,resolutionFingerprint)))).ToLowerInvariant();

                return new QualityFindingEvidenceReplayDescriptor(
                    item.FindingId,
                    item.EvidenceHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal).ToArray(),
                    resolutionFingerprint,
                    descriptorFingerprint);
            })
            .ToArray();
    }

    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions,
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> descriptors)
    {
        var errors=new List<string>(QualityFindingEvidenceResolutionRuntime.Validate(run,bindings,resolutions));
        var expected=Create(run,bindings,resolutions);
        var actual=descriptors.OrderBy(item=>item.FindingId.Value,StringComparer.Ordinal).ToArray();

        if(expected.Count!=actual.Length)
            errors.Add("Quality finding evidence replay descriptor count must match.");

        var count=Math.Min(expected.Count,actual.Length);
        for(var index=0;index<count;index++)
        {
            var left=expected[index];
            var right=actual[index];
            if(right.FindingId!=left.FindingId)
                errors.Add($"Replay descriptor {index} finding identity mismatch.");
            if(!right.EvidenceHandles.SequenceEqual(left.EvidenceHandles))
                errors.Add($"Replay descriptor {index} Evidence handle set mismatch.");
            if(right.ResolutionFingerprint!=left.ResolutionFingerprint)
                errors.Add($"Replay descriptor {index} resolution fingerprint mismatch.");
            if(right.DescriptorFingerprint.Length!=64 || !right.DescriptorFingerprint.All(Uri.IsHexDigit))
                errors.Add($"Replay descriptor {index} descriptor fingerprint malformed.");
            if(right.DescriptorFingerprint!=left.DescriptorFingerprint)
                errors.Add($"Replay descriptor {index} descriptor fingerprint mismatch.");
        }

        if(actual.Select(item=>item.FindingId).Distinct().Count()!=actual.Length)
            errors.Add("Replay descriptor finding identities must be unique.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions,
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> descriptors)=>
        Validate(run,bindings,resolutions,descriptors).Count==0;

    public static bool IsEquivalent(
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> left,
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> right)=>
        left.OrderBy(item=>item.FindingId.Value,StringComparer.Ordinal)
            .Select(item=>item.DescriptorFingerprint)
            .SequenceEqual(right.OrderBy(item=>item.FindingId.Value,StringComparer.Ordinal)
                .Select(item=>item.DescriptorFingerprint));
}
