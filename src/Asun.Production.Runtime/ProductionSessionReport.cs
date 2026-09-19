namespace Asun.Production.Runtime;

public sealed record ProductionSessionReport(
    Guid SessionId,
    string ProgramFingerprint,
    int FrameCount,
    IReadOnlyList<ProductionFrameExecution> Frames,
    string Fingerprint);
