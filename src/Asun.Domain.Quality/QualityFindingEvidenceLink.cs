namespace Asun.Domain.Quality;

/// <summary>
/// Neutral link between a quality finding and an evidence stable key.
/// The key is intentionally opaque; evidence storage/format belongs to the platform layer.
/// </summary>
public sealed record QualityFindingEvidenceLink(
    QualityFindingId FindingId,
    QualityEvidenceKey EvidenceKey)
{
    public bool IsValid =>
        FindingId.IsValid &&
        EvidenceKey.IsValid;
}
