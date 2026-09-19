namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogStatistics(
    int DescriptorCount,
    IReadOnlyList<EvidenceCatalogKindCount> KindCounts,
    IReadOnlyList<EvidenceCatalogMediaTypeCount> MediaTypeCounts,
    long KnownByteLengthTotal);
