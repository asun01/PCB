namespace Asun.Domain.Pcb;

public sealed record PcbAssemblySnapshot(
    PcbBoardDefinition Board,
    IReadOnlyList<PcbComponentReference> Components,
    PcbComponentStatistics Statistics,
    string Fingerprint);
