namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualityFindingSelection(
    string? FindingId,
    long SelectionSequence,
    string StatusText);

public static class ClientQualityFindingSelectionRuntime
{
    public static ClientQualityFindingSelection CreateInitial() =>
        new(null,0,"No Quality finding selected.");

    public static ClientQualityFindingSelection Select(
        IReadOnlyList<ClientQualityFindingDisplayItem> items,
        string findingId,
        long previousSelectionSequence)
    {
        ArgumentNullException.ThrowIfNull(items);

        if(string.IsNullOrWhiteSpace(findingId))
            throw new ArgumentException("Finding id cannot be blank.",nameof(findingId));
        if(previousSelectionSequence<0)
            throw new ArgumentOutOfRangeException(nameof(previousSelectionSequence));

        var item=items.FirstOrDefault(candidate=>candidate.FindingId==findingId);
        if(item is null)
            return new(
                null,
                previousSelectionSequence,
                $"Finding {findingId} is not available in the current Quality projection.");

        return new(
            item.FindingId,
            checked(previousSelectionSequence+1),
            $"Finding {item.FindingId} · Rule {item.RuleCode} · {item.Outcome} · {item.Severity} · Evidence {item.EvidenceCount}.");
    }
}
