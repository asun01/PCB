using Asun.Domain.Quality;
using Asun.Platform.Evidence;

namespace Asun.Platform.QualityEvidenceIntegration;

public static class QualityEvidenceHandleBindingRuntime
{
    public static IReadOnlyList<QualityEvidenceHandleBinding> Create(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(bindings);

        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            throw new ArgumentException("Quality inspection run is invalid.",nameof(run));

        var links=run.Results
            .SelectMany(result=>result.Evidence.Links)
            .ToArray();

        if(links.Length!=bindings.Count)
            throw new ArgumentException("Evidence binding count must match Quality evidence links.",nameof(bindings));

        var sortedLinks=links
            .OrderBy(link=>link.FindingId.Value,StringComparer.Ordinal)
            .ThenBy(link=>link.EvidenceKey.Value,StringComparer.Ordinal)
            .ToArray();
        var sortedBindings=bindings
            .OrderBy(binding=>binding.FindingId.Value,StringComparer.Ordinal)
            .ThenBy(binding=>binding.EvidenceKey.Value,StringComparer.Ordinal)
            .ToArray();
        var result=new List<QualityEvidenceHandleBinding>(bindings.Count);

        for(var index=0;index<sortedBindings.Length;index++)
        {
            var link=sortedLinks[index];
            var binding=sortedBindings[index];

            if(!binding.FindingId.IsValid || !binding.EvidenceKey.IsValid || !binding.EvidenceHandle.IsValid)
                throw new ArgumentException($"Quality evidence binding {index} is invalid.",nameof(bindings));

            if(binding.FindingId!=link.FindingId || binding.EvidenceKey!=link.EvidenceKey)
                throw new ArgumentException("Quality evidence binding does not match the Quality evidence link.",nameof(bindings));

            result.Add(binding);
        }

        if(result.Select(binding=>binding.EvidenceHandle).Distinct().Count()!=result.Count)
            throw new ArgumentException("Evidence handles must be unique within the Quality binding projection.",nameof(bindings));

        return result;
    }

    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(bindings);

        var errors=new List<string>();
        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            errors.Add("Quality inspection run is invalid.");

        var links=run.Results
            .SelectMany(result=>result.Evidence.Links)
            .OrderBy(link=>link.FindingId.Value,StringComparer.Ordinal)
            .ThenBy(link=>link.EvidenceKey.Value,StringComparer.Ordinal)
            .ToArray();
        var ordered=bindings
            .OrderBy(binding=>binding.FindingId.Value,StringComparer.Ordinal)
            .ThenBy(binding=>binding.EvidenceKey.Value,StringComparer.Ordinal)
            .ToArray();
        if(links.Length!=ordered.Length)
            errors.Add("Quality evidence binding count must match Quality evidence links.");

        var count=Math.Min(links.Length,ordered.Length);
        for(var index=0;index<count;index++)
        {
            var link=links[index];
            var binding=ordered[index];
            if(!binding.FindingId.IsValid || !binding.EvidenceKey.IsValid || !binding.EvidenceHandle.IsValid)
                errors.Add($"Quality evidence binding {index} is invalid.");
            if(binding.FindingId!=link.FindingId || binding.EvidenceKey!=link.EvidenceKey)
                errors.Add($"Quality evidence binding {index} does not match its Quality link.");
        }

        if(ordered.Select(binding=>binding.EvidenceHandle).Distinct().Count()!=ordered.Length)
            errors.Add("Evidence handles must be unique within the Quality binding projection.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings)=>
        Validate(run,bindings).Count==0;
}
