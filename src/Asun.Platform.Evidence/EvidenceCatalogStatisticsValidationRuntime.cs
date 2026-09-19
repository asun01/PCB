namespace Asun.Platform.Evidence;

public static class EvidenceCatalogStatisticsValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(statistics);

        var errors=new List<string>();

        errors.AddRange(EvidenceCatalogSnapshotValidationRuntime.Validate(snapshot));

        if(statistics.DescriptorCount!=snapshot.Count)
            errors.Add("Evidence catalog statistics descriptor count must match the snapshot.");

        if(statistics.KnownByteLengthTotal<0)
            errors.Add("Evidence catalog statistics known byte length total must be non-negative.");

        if(statistics.KindCounts.Any(item=>
            !Enum.IsDefined(item.Kind) ||
            item.Count<=0))
        {
            errors.Add("Evidence catalog kind counts must use defined kinds and positive counts.");
        }

        if(statistics.MediaTypeCounts.Any(item=>
            string.IsNullOrWhiteSpace(item.MediaType) ||
            item.Count<=0))
        {
            errors.Add("Evidence catalog media type counts must use non-blank media types and positive counts.");
        }

        if(statistics.KindCounts.Select(item=>item.Kind).Distinct().Count()!=statistics.KindCounts.Count)
            errors.Add("Evidence catalog kind counts must be unique by kind.");

        if(statistics.MediaTypeCounts.Select(item=>item.MediaType).Distinct(StringComparer.Ordinal).Count()!=statistics.MediaTypeCounts.Count)
            errors.Add("Evidence catalog media type counts must be unique by media type.");

        if(statistics.KindCounts.OrderBy(item=>item.Kind).Select(item=>item.Kind).SequenceEqual(
            statistics.KindCounts.Select(item=>item.Kind))==false)
        {
            errors.Add("Evidence catalog kind counts must use canonical ordering.");
        }

        if(statistics.MediaTypeCounts.OrderBy(item=>item.MediaType,StringComparer.Ordinal).Select(item=>item.MediaType).SequenceEqual(
            statistics.MediaTypeCounts.Select(item=>item.MediaType))==false)
        {
            errors.Add("Evidence catalog media type counts must use canonical ordering.");
        }

        if(statistics.KindCounts.Sum(item=>item.Count)!=snapshot.Count)
            errors.Add("Evidence catalog kind counts must cover all descriptors.");

        if(statistics.MediaTypeCounts.Sum(item=>item.Count)!=snapshot.Count)
            errors.Add("Evidence catalog media type counts must cover all descriptors.");

        var expected=EvidenceCatalogStatisticsRuntime.Create(snapshot);

        if(statistics.KnownByteLengthTotal!=expected.KnownByteLengthTotal)
            errors.Add("Evidence catalog known byte length total does not match the snapshot.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogStatistics statistics)=>
        Validate(snapshot,statistics).Count==0;
}
