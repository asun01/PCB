using Asun.Domain.Quality;
using Asun.Platform.Evidence;

namespace Asun.Platform.QualityEvidenceIntegration;

public sealed record QualityEvidenceHandleBinding(
    QualityFindingId FindingId,
    QualityEvidenceKey EvidenceKey,
    EvidenceHandle EvidenceHandle);
