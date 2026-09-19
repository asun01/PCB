namespace Asun.Platform.Evidence;

public sealed record EvidenceDescriptor(
    EvidenceHandle Handle,
    EvidenceKind Kind,
    string MediaType,
    long? ByteLength,
    string? DisplayName);
