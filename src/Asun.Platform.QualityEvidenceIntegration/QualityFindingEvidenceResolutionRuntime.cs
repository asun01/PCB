using Asun.Domain.Quality;
using Asun.Platform.Evidence;

namespace Asun.Platform.QualityEvidenceIntegration;

public sealed record QualityFindingEvidenceResolution(
    QualityFindingId FindingId,
    IReadOnlyList<EvidenceHandle> EvidenceHandles);

public static class QualityFindingEvidenceResolutionRuntime
{
    public static IReadOnlyList<QualityFindingEvidenceResolution> Create(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(bindings);

        var validation=QualityEvidenceHandleBindingRuntime.Validate(run,bindings);
        if(validation.Count>0)
            throw new ArgumentException(
                string.Join(" ",validation),
                nameof(bindings));

        var byFinding=bindings
            .GroupBy(binding=>binding.FindingId)
            .OrderBy(group=>group.Key.Value,StringComparer.Ordinal)
            .Select(group=>
                new QualityFindingEvidenceResolution(
                    group.Key,
                    group
                        .Select(binding=>binding.EvidenceHandle)
                        .Distinct()
                        .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
                        .ToArray()))
            .ToArray();

        return byFinding;
    }

    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(resolutions);

        var errors=new List<string>();
        var bindingErrors=QualityEvidenceHandleBindingRuntime.Validate(run,bindings);
        errors.AddRange(bindingErrors);

        var expected=CreateExpected(bindings);
        var actual=resolutions
            .OrderBy(resolution=>resolution.FindingId.Value,StringComparer.Ordinal)
            .ToArray();

        if(expected.Count!=actual.Length)
            errors.Add("Finding evidence resolution count must match bound findings.");

        var count=Math.Min(expected.Count,actual.Length);
        for(var index=0;index<count;index++)
        {
            var left=expected[index];
            var right=actual[index];

            if(!right.FindingId.IsValid)
                errors.Add($"Finding evidence resolution {index} has an invalid finding id.");
            if(right.FindingId!=left.FindingId)
                errors.Add($"Finding evidence resolution {index} has a mismatched finding id.");
            if(!right.EvidenceHandles.SequenceEqual(left.EvidenceHandles))
                errors.Add($"Finding evidence resolution {index} does not match bound opaque Evidence handles.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions)=>
        Validate(run,bindings,resolutions).Count==0;

    private static IReadOnlyList<QualityFindingEvidenceResolution> CreateExpected(
        IReadOnlyList<QualityEvidenceHandleBinding> bindings)=>
        bindings
            .GroupBy(binding=>binding.FindingId)
            .OrderBy(group=>group.Key.Value,StringComparer.Ordinal)
            .Select(group=>
                new QualityFindingEvidenceResolution(
                    group.Key,
                    group
                        .Select(binding=>binding.EvidenceHandle)
                        .Distinct()
                        .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
                        .ToArray()))
            .ToArray();
}
