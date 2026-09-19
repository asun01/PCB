namespace Asun.Validation;

public sealed record StageArtifactReport(
    bool IsValid,
    int LoopGroups,
    int CheckCalls,
    int RoundAssertions,
    bool BalancedDelimiters,
    bool HasPlaceholderMarkers,
    IReadOnlyList<string> Errors);
