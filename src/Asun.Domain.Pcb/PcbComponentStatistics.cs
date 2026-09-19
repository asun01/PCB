namespace Asun.Domain.Pcb;

public sealed record PcbComponentStatistics(
    int ComponentCount,
    int TopCount,
    int BottomCount,
    int InternalCount);
