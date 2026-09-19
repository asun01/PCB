using Asun.Platform.Evidence;

namespace Asun.Production.Runtime;

public sealed record ProductionEvidenceFrameReference(
    long Sequence,
    IReadOnlyList<EvidenceHandle> Handles);
