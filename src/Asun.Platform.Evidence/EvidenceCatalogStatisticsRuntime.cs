namespace Asun.Platform.Evidence;

public static class EvidenceCatalogStatisticsRuntime
{
    public static EvidenceCatalogStatistics Create(
        EvidenceCatalogSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Evidence catalog snapshot is invalid.",
                nameof(snapshot));

        var kindCounts=snapshot.Descriptors
            .GroupBy(descriptor=>descriptor.Kind)
            .OrderBy(group=>group.Key)
            .Select(group=>new EvidenceCatalogKindCount(
                group.Key,
                group.Count()))
            .ToArray();

        var mediaTypeCounts=snapshot.Descriptors
            .GroupBy(descriptor=>descriptor.MediaType,StringComparer.Ordinal)
            .OrderBy(group=>group.Key,StringComparer.Ordinal)
            .Select(group=>new EvidenceCatalogMediaTypeCount(
                group.Key,
                group.Count()))
            .ToArray();

        var knownByteLengthTotal=snapshot.Descriptors
            .Where(descriptor=>descriptor.ByteLength is not null)
            .Select(descriptor=>descriptor.ByteLength!.Value)
            .Aggregate(0L,(total,value)=>checked(total+value));

        return new EvidenceCatalogStatistics(
            snapshot.Count,
            kindCounts,
            mediaTypeCounts,
            knownByteLengthTotal);
    }
}
