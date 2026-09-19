namespace Asun.Domain.Quality;

public sealed record QualityInspectionAuditDiff(
    bool ResultIdentityChanged,
    bool SnapshotIdentityChanged,
    bool SequenceChanged,
    bool FindingCountChanged,
    bool EvidenceLinkCountChanged,
    bool ContentFingerprintChanged)
{
    public bool IsEmpty =>
        !ResultIdentityChanged &&
        !SnapshotIdentityChanged &&
        !SequenceChanged &&
        !FindingCountChanged &&
        !EvidenceLinkCountChanged &&
        !ContentFingerprintChanged;
}
